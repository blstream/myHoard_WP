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

        public Dictionary<Media,BitmapImage> PictureDictionary(int itemId)
        {
            Dictionary<Media, BitmapImage> pictureDictionary = new Dictionary<Media, BitmapImage>();

            foreach(Media m in MediaList(itemId))
            {
                pictureDictionary.Add(m, GetImageFromIsolatedStorage(m));
            }

            return pictureDictionary;
        }

        public void SavePictureDicitonary(Dictionary<Media,BitmapImage> mediaDictionary)
        {
            foreach(Media m in mediaDictionary.Keys)
            {
                if(String.IsNullOrEmpty(m.FileName))
                {
                    AddMedia(SaveImageToIsolatedStorage(mediaDictionary[m], m));
                }
            }
        }

        public Media SaveImageToIsolatedStorage(BitmapImage image, Media media)
        {
            
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            media.FileName = Guid.NewGuid().ToString()+".jpg";

            using (IsolatedStorageFileStream isostream = isf.CreateFile(media.FileName))
            {
                WriteableBitmap wb = new WriteableBitmap(image);
                Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                isostream.Close();
            }
            return media;
        }

        public BitmapImage GetImageFromIsolatedStorage(Media media)
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
