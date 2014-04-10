﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models
{
    public class Item : BaseEntity
    {
        private string name;
        private string description;
        private int quantity;
        private int collectionId;
        private float locationLat;
        private float locationLng;
        private DateTime createdDate;
        private DateTime modifiedDate;


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
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

        public float LocationLat
        {
            get { return locationLat; }
            set
            {
                locationLat = value;
                ModifiedDate = DateTime.Now;
                IsSynced = false;
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
