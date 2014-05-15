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
                eventAggregator.Publish(new ServiceErrorMessage(AppResources.DuplicateNameErrorMessage));
                return collection;
            }
                
        }

        public void DeleteCollection(Collection collection, bool forceDelete=false)
        {
            if (forceDelete || collection.IsPrivate || String.IsNullOrEmpty(collection.ServerId))
            {
                databaseService.Delete(collection);
            }
            else
            {
                collection.ToDelete = true;
                ModifyCollection(collection);
            }

            ItemService itemService = IoC.Get<ItemService>();
            foreach (Item i in itemService.ItemList(collection.Id))
            {
                itemService.DeleteItem(i, true);
            }

            
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
                eventAggregator.Publish(new ServiceErrorMessage(AppResources.DuplicateNameErrorMessage));
                return col;
            }
        }

        public Collection GetCollection(int id)
        {
            return databaseService.Get<Collection>(id);
        }

        
        public List<Collection> CollectionList(bool withDeleted=false, bool withThumbnails = false)
        {
            List<Collection> collectionList = withDeleted? databaseService.ListAll<Collection>():
                databaseService.ListAll<Collection>().Where(x=>x.ToDelete==false).ToList();
            if(withThumbnails)
            {
                MediaService ms = IoC.Get<MediaService>();
                foreach (Collection c in collectionList)
                {
                    c.Thumbnail = ms.GetRandomThumbnail(c);
                }
            }

            return collectionList;
        }

        public void CloseConnection()
        {
            databaseService.CloseConnection();
        }

        public int DeleteAll()
        {
            return databaseService.DeleteAll<Collection>();
        }
    }
}
