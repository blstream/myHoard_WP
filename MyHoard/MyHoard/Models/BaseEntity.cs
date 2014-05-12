using Caliburn.Micro;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models
{
    public class BaseEntity : PropertyChangedBase
    {
        private int id;
        private string serverId;
        private bool isSynced;
        private bool toDelete;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public bool ToDelete
        {
            get { return toDelete; }
            set { toDelete = value; }
        }

        public string ServerId
        {
            get { return serverId; }
            set
            {
                serverId = value;
            }
        }


        public bool IsSynced
        {
            get { return isSynced; }
            set
            {
                isSynced = value;
            }
        }

    }
}
