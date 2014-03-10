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

        public void DeleteItem(Item item)
        {
            MediaService ms = IoC.Get<MediaService>();
            foreach(Media m in ms.MediaList(item.Id, false))
            {
                m.ToDelete = true;
                ms.ModifyMedia(m);
            }
            databaseService.Delete(item);
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

        public List<Item> ItemList(int collectionId)
        {
            return databaseService.ListAllTable<Item>().Where(i => i.CollectionId == collectionId).ToList();
        }

    }
}
