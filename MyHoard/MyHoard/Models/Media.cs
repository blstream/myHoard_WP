using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyHoard.Models
{
    public class Media: BaseEntity
    {
        private string fileName;
        private DateTime createdDate;
        private int itemId;
        private WriteableBitmap image;

        public Media()
        {
            CreatedDate = DateTime.Now;
        }

        [Ignore]
        public WriteableBitmap Image
        {
            get { return image; }
            set
            {
                image = value;
            }
        }

        public DateTime CreatedDate
        {
            get { return createdDate; }
            set
            {
                createdDate = value;
            }
        }

        [Indexed]
        public int ItemId
        {
            get { return itemId; }
            set
            {
                itemId = value;
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
            }
        }

    }
}
