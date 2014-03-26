using Caliburn.Micro;
using Microsoft.Phone;
using MyHoard.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MyHoard.Services
{
    public class MediaService
    {
        private const int ThumbnailSize = 100;
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

        public List<Media> MediaList(bool withPictures, bool thumbnail = false)
        {
            List<Media> mediaList = databaseService.ListAll<Media>();
            if (withPictures)
            {
                foreach (Media m in mediaList)
                {
                    m.Image = GetPictureFromIsolatedStorage(m, thumbnail);
                }
            }

            return mediaList;


        }

        public List<Media> MediaList(int itemId, bool withPictures, bool thumbnail = false)
        {
            List<Media> mediaList = databaseService.ListAllTable<Media>().Where(i => i.ItemId == itemId).ToList();

            if (withPictures)
            {
                mediaList = mediaList.Where(i => i.ToDelete == false).ToList();
                foreach (Media m in mediaList)
                {
                    m.Image = GetPictureFromIsolatedStorage(m, thumbnail);
                }
            }

            return mediaList;
        }

        public List<string> MediaStringList(int itemId, string backend)
        {
            List<Media> mediaList = databaseService.ListAllTable<Media>().Where(i => i.ItemId == itemId && i.ToDelete == false).ToList();
            List<string> ids = new List<string>();

            foreach (Media m in mediaList)
            {
                switch (backend)
                {
                    case "Python":
                        ids.Add(m.PythonId);
                        break;
                    case "Java1":
                        ids.Add(m.Java1Id);
                        break;
                    case "Java2":
                        ids.Add(m.Java2Id);
                        break;
                }
            }
            return ids;
        }


        public void SavePictureList(IList<Media> pictureList)
        {
            bool datachanged = false;
            int parentId = 0;
            foreach (Media m in pictureList)
            {
                if (String.IsNullOrEmpty(m.FileName) && !m.ToDelete)
                {
                    AddMedia(SavePictureToIsolatedStorage(m));
                    datachanged = true;
                    parentId = m.ItemId;
                }
                else if (!String.IsNullOrEmpty(m.FileName) && m.ToDelete)
                {
                    ModifyMedia(m);
                    datachanged = true;
                    parentId = m.ItemId;
                }
            }
            if (datachanged)
            {
                ItemService itemService = IoC.Get<ItemService>();
                Item i = itemService.GetItem(parentId);
                i.Desync();
                itemService.ModifyItem(i);
            }
        }

        public void CleanIsolatedStorage()
        {
            foreach (Media m in MediaList(false, false))
            {
                if (m.ToDelete)
                {
                    DeleteMediaFromIsolatedStorage(m);
                    if (String.IsNullOrEmpty(m.PythonId) && String.IsNullOrEmpty(m.Java1Id) && String.IsNullOrEmpty(m.Java2Id))
                        DeleteMedia(m);
                }
            }
        }

        public WriteableBitmap ByteArrayToWriteableBitmap(byte[] array)
        {
            MemoryStream stream = new MemoryStream(array);
            WriteableBitmap bmp = PictureDecoder.DecodeJpeg(stream);
            return bmp;
        }

        public Media SavePictureToIsolatedStorage(Media media)
        {
            media.FileName = Guid.NewGuid().ToString() + ".jpg";

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

        public byte[] GetPictureAsByteArray(Media media)
        {
            try
            {
                using (IsolatedStorageFileStream isfs = isolatedStorageFile.OpenFile(media.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[isfs.Length];
                    isfs.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }

        }

        public string GetAbsolutePath(string filename)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            string absoulutePath = null;

            if (isoStore.FileExists(filename))
            {
                IsolatedStorageFileStream output = new IsolatedStorageFileStream(filename, FileMode.Open, isoStore);
                absoulutePath = output.Name;

                output.Close();
                output = null;
            }

            return absoulutePath;
        }


        public WriteableBitmap GetPictureFromIsolatedStorage(Media media, bool thumbnail = false)
        {
            try
            {
                using (IsolatedStorageFileStream isfs = isolatedStorageFile.OpenFile(media.FileName, FileMode.Open, FileAccess.Read))
                {
                    WriteableBitmap wb;
                    wb = (thumbnail) ? PictureDecoder.DecodeJpeg(isfs, ThumbnailSize, ThumbnailSize) : PictureDecoder.DecodeJpeg(isfs);
                    isfs.Close();
                    return wb;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public void DeleteAll()
        {
            foreach (Media m in MediaList(false))
            {
                m.ToDelete = true;
                m.PythonId = null;
                m.Java1Id = null;
                m.Java2Id = null;
                ModifyMedia(m);
            }
        }

    }
}
