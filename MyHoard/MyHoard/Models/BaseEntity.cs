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
        private string pythonId;
        private string java1Id;
        private string java2Id;
        private bool pythonIsSynced;
        private bool java1IsSynced;
        private bool java2IsSynced;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public string PythonId
        {
            get { return pythonId; }
            set
            {
                pythonId = value;
            }
        }


        public string Java1Id
        {
            get { return java1Id; }
            set
            {
                java1Id = value;
            }
        }


        public string Java2Id
        {
            get { return java2Id; }
            set
            {
                java2Id = value;
            }
        }

        
        public bool PythonIsSynced
        {
            get { return pythonIsSynced; }
            set
            {
                pythonIsSynced = value;
            }
        }

        
        public bool Java1IsSynced
        {
            get { return java1IsSynced; }
            set
            {
                java1IsSynced = value;
            }
        }

        
        public bool Java2IsSynced
        {
            get { return java2IsSynced; }
            set
            {
                java2IsSynced = value;
            }
        }

        protected void Desync()
        {
            pythonIsSynced = false;
            Java1IsSynced = false;
            Java2IsSynced = false;
        }
    }
}
