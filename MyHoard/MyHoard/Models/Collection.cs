using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models
{
    public class Collection : BaseEntity
    {
        private string name;
        private string description;
        private string thumbnail;
        private string tags;
        private int itemsNumber;
        private DateTime createdDate;
        private DateTime modifiedDate;

        public Collection()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                ModifiedDate = DateTime.Now;
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public int ItemsNumber
        {
            get { return itemsNumber; }
            set
            {
                itemsNumber = value;
                ModifiedDate = DateTime.Now;
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
