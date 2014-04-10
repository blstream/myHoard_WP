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
    public class ItemService
    {
        private DatabaseService databaseService;
        
        public ItemService()
        {
            databaseService = IoC.Get<DatabaseService>();
        }

        public Item AddItem(Item item)
        {
            if (ItemList(item.CollectionId).Count(i => i.Name == item.Name) == 0)
            {
                return databaseService.Add(item);
            }
            else
            {
                IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
                eventAggregator.Publish(new ServiceErrorMessage(AppResources.DuplicateNameErrorMessage));
                return item;
            }
        }

        public void DeleteItem(Item item, bool forceDelete = false)
        {

            if(forceDelete || String.IsNullOrEmpty(item.ServerId))
            {
                databaseService.Delete(item);
            }
            else
            {
                item.ToDelete = true;
                ModifyItem(item);
            }

            MediaService ms = IoC.Get<MediaService>();
            foreach(Media m in ms.MediaList(item.Id, false, false))
            {
                m.ServerId = null;
                m.ToDelete = true;
                ms.ModifyMedia(m);
            }
            
        }

        public int DeleteAll()
        {
            return databaseService.DeleteAll<Item>();
        }

        public Item ModifyItem(Item item)
        {
            Item it = ItemList(item.CollectionId).FirstOrDefault(i => i.Name == item.Name);
            if (it == null || it.Id == item.Id)
            {
                return databaseService.Modify(item);
            }
            else
            {
                IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
                eventAggregator.Publish(new ServiceErrorMessage(AppResources.DuplicateNameErrorMessage));
                return it;
            }
        }

        public Item GetItem(int id)
        {
            return databaseService.Get<Item>(id);
        }


        public List<Item> ItemList()
        {
            return databaseService.ListAll<Item>();
        }

        public List<Item> ItemList(int collectionId, bool withDeleted=false)
        {
            if (withDeleted)
                return databaseService.ListAllTable<Item>().Where(i => i.CollectionId == collectionId).ToList();
            else
                return databaseService.ListAllTable<Item>().Where(i => (i.CollectionId == collectionId && i.ToDelete == false)).ToList();
        }

    }
}
