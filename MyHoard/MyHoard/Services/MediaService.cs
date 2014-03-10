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
        private IsolatedStorageFile isolatedStorageFile;

        public MediaService()
        {
            databaseService = IoC.Get<DatabaseService>();
            isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
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

        public List<Media> MediaList(bool withPictures)
        {
            List<Media> mediaList = databaseService.ListAll<Media>();
            if (withPictures)
            {
                foreach (Media m in mediaList)
                {
                    m.Image = GetPictureFromIsolatedStorage(m);
                }
            }

            return mediaList;
            
            
        }

        public List<Media> MediaList(int itemId, bool withPictures)
        {
            List<Media> mediaList = databaseService.ListAllTable<Media>().Where(i => i.ItemId == itemId).ToList();

            if (withPictures)
            {
                mediaList = mediaList.Where(i => i.ToDelete == false).ToList();
                foreach (Media m in mediaList)
                {
                    m.Image = GetPictureFromIsolatedStorage(m);
                }
            }

            return mediaList;
        }

        
        public void SavePictureList(IList<Media> pictureList)
        {
            foreach(Media m in pictureList)
            {
                if(String.IsNullOrEmpty(m.FileName))
                {
                    AddMedia(SavePictureToIsolatedStorage(m));
                }
                if(m.ToDelete)
                {
                    ModifyMedia(m);
                }
            }
        }

        public void CleanIsolatedStorage()
        {
            foreach(Media m in MediaList(false))
            {
                if(m.ToDelete)
                {
                    DeleteMediaFromIsolatedStorage(m);
                    DeleteMedia(m);
                }
            }
        }

        public Media SavePictureToIsolatedStorage(Media media)
        {
            media.FileName = Guid.NewGuid().ToString()+".jpg";

            using (IsolatedStorageFileStream isostream = isolatedStorageFile.CreateFile(media.FileName))
            {
                WriteableBitmap wb = new WriteableBitmap(media.Image);
                Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                isostream.Close();
            }
            return media;
        }

        public void DeleteMediaFromIsolatedStorage(Media media)
        {
            if (isolatedStorageFile.FileExists(media.FileName))
            {
                isolatedStorageFile.DeleteFile(media.FileName);
            }
        }

        public BitmapImage GetPictureFromIsolatedStorage(Media media)
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
