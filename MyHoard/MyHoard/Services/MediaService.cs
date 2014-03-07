using Caliburn.Micro;
using MyHoard.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyHoard.Services
{
    public class MediaService
    {
        private DatabaseService databaseService;

        public MediaService()
        {
            databaseService = IoC.Get<DatabaseService>();
        }

        public Media AddMedia(Media media)
        {
            return databaseService.Add(media);
        }

        public void DeleteMedia(Media media)
        {
            databaseService.Delete(media);
        }

        public Media ModifyMedia(Media media)
        {
            return databaseService.Modify(media);
        }

        public List<Media> MediaList()
        {
            return databaseService.ListAll<Media>();
        }

        public List<Media> MediaList(int itemId)
        {
            return databaseService.ListAllTable<Media>().Where(i => i.ItemId == itemId).ToList();
        }

        public void SaveToIsolatedStorage(BitmapImage image, int itemId)
        {
            Media m = new Media() { FileName = Guid.NewGuid().ToString(), ItemId = itemId };
            
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

            using (IsolatedStorageFileStream isostream = isf.CreateFile(m.FileName))
            {
                WriteableBitmap wb = new WriteableBitmap(image);
                Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                isostream.Close();
            }
        }

        public BitmapImage GetFromIsolatedStorage(Media media)
        {
            byte[] data;

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isfs = isf.OpenFile(media.FileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[isfs.Length];
                        isfs.Read(data, 0, data.Length);
                        isfs.Close();
                    }
                }

                MemoryStream ms = new MemoryStream(data);
                BitmapImage bi = new BitmapImage();
                
                bi.SetSource(ms);
                return bi;
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            } 
        }

    }
}
