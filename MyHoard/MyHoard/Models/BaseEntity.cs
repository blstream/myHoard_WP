using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models
{
    public class BaseEntity
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        private string serverId;
        public string ServerId
        {
            get { return serverId; }
            set
            {
                serverId = value;
            }
        }
    }
}
