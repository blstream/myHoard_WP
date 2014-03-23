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

        public string GetServerId(string backend)
        {
            string serverid = "";
            switch (backend)
            {
                case "Python":
                    serverid = PythonId;
                    break;
                case "Java1":
                    serverid = Java1Id;
                    break;
                case "Java2":
                    serverid = Java2Id;
                    break;
            }
            return serverid;
        }
        public void SetServerId(string id, string backend)
        {
            switch (backend)
            {
                case "Python":
                    PythonId = id;
                    break;
                case "Java1":
                    Java1Id = id;
                    break;
                case "Java2":
                    Java2Id = id;
                    break;
            }
        }

        public bool IsSynced(string backend)
        {
            bool synced=false;
            switch (backend)
            {
                case "Python":
                    synced = PythonIsSynced;
                    break;
                case "Java1":
                    synced = Java1IsSynced;
                    break;
                case "Java2":
                    synced = Java2IsSynced;
                    break;
            }
            return synced;
        }

        public void SetSynced(bool synced, string backend)
        {
            switch (backend)
            {
                case "Python":
                    PythonIsSynced = synced;
                    break;
                case "Java1":
                    Java1IsSynced = synced;
                    break;
                case "Java2":
                    Java2IsSynced = synced;
                    break;
            }
        }

        public bool ToDelete
        {
            get { return toDelete; }
            set { toDelete = value; }
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

        public void Desync()
        {
            pythonIsSynced = false;
            Java1IsSynced = false;
            Java2IsSynced = false;
        }
    }
}
