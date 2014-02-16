using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Resources;
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
            databaseService = IoC.Get<DatabaseService>();
        }

        public Collection AddCollection(Collection collection)
        {                       
            if(CollectionList().Count(c=>c.Name==collection.Name)==0)
            {
                return databaseService.Add(collection);
            }
            else
            {
                IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
                eventAggregator.Publish(new CollectionServiceErrorMessage(AppResources.DuplicateNameErrorMessage));
                return collection;
            }
                
        }

        public void DeleteCollection(Collection collection)
        {
            databaseService.Delete(collection);
        }

        public Collection ModifyCollection(Collection collection)
        {
            Collection col = CollectionList().FirstOrDefault(c => c.Name == collection.Name);
            if (col == null || col.Id == collection.Id)
            {
                return databaseService.Modify(collection);
            }
            else
            {
                IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
                eventAggregator.Publish(new CollectionServiceErrorMessage(AppResources.DuplicateNameErrorMessage));
                return col;
            }
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
