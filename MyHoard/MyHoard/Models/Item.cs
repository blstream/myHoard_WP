using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyHoard.Models
{
    public class Item : BaseEntity
    {
        private string name = string.Empty;
        private string description = string.Empty;
        private int quantity;
        private int collectionId;
        private bool locationSet;
        private float locationLat;
        private float locationLng;
        private string locationName;
        private DateTime createdDate;
        private DateTime modifiedDate;
        private ImageSource thumbnail;

        public Item()
        {
            CreatedDate = DateTime.Now;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                IsSynced = false;
                NotifyOfPropertyChange(() => Name);
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                IsSynced = false;
                NotifyOfPropertyChange(() => Description);
            }
        }

        [Ignore]
        public ImageSource Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
            }
        }

        [Indexed]
        public int CollectionId
        {
            get { return collectionId; }
            set
            {
                collectionId = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public bool LocationSet
        {
            get { return locationSet; }
            set
            {
                locationSet = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
                NotifyOfPropertyChange(() => LocationSet);
            }
        }

        public float LocationLat
        {
            get { return locationLat; }
            set
            {
                locationLat = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
                NotifyOfPropertyChange(() => LocationLat);
            }
        }

        public float LocationLng
        {
            get { return locationLng; }
            set
            {
                locationLng = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
                NotifyOfPropertyChange(() => LocationLng);
            }
        }

        public string LocationName
        {
            get { return locationName; }
            set
            {
                locationName = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
                NotifyOfPropertyChange(() => LocationName);
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

        public DateTime ModifiedDate
        {
            get { return modifiedDate; }
            set
            {
                modifiedDate = value;
            }
        }


    }
}
