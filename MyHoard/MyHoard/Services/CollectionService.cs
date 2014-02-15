using MyHoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class CollectionService
    {
        private DatabaseService databaseService;


        public CollectionService()
        {
            databaseService=new DatabaseService();

        }

        public Collection AddCollection(Collection collection)
        {
            return databaseService.Add(collection);
        }

        public void DeleteCollection(Collection collection)
        {
            databaseService.Delete(collection);
        }

        public Collection ModifyCollection(Collection collection)
        {
            return databaseService.Modify(collection);
        }

        public Collection GetCollection(int id)
        {
            return databaseService.Get<Collection>(id);
        }

        
        public List<Collection> CollectionList()
        {
            return databaseService.ListAll<Collection>();
        }

        public void CloseConnection()
        {
            databaseService.CloseConnection();
        }
    }
}
