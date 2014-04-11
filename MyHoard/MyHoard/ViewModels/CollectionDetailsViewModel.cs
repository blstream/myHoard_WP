using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.ViewModels
{
    public class CollectionDetailsViewModel : ViewModelBase
    {
        private ItemService itemService;
        private Collection currentCollection;
        private String collectionName;
        private int collectionId;
        private Item selectedItem;
        private List<Item> items;
        

                
        public CollectionDetailsViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService) : base(navigationService,collectionService)
        {
            this.itemService = itemService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Items = itemService.ItemList(CollectionId, false, true);
        }

        public List<Item> Items
        {
            get { return items; }
            set 
            { 
                items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            { 
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public Collection CurrentCollection
        {
            get { return currentCollection; }
            set 
            { 
                currentCollection = value;
                NotifyOfPropertyChange(() => CurrentCollection);
                CollectionName = CurrentCollection.Name;
            }
        }

        public String CollectionName
        {
            get { return collectionName; }
            set 
            { 
                collectionName = value;
                NotifyOfPropertyChange(() => CollectionName);
            }
        }

        public int CollectionId
        {
            get { return collectionId; }
            set
            {
                collectionId = value;
                NotifyOfPropertyChange(() => CollectionId);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CurrentCollection = CollectionService.GetCollection(CollectionId);
        }

        public void Edit()
        {
            NavigationService.UriFor<AddCollectionViewModel>().WithParam(x => x.CollectionId, CurrentCollection.Id).Navigate();
        }

        public void AddItem()
        {
            NavigationService.UriFor<AddItemViewModel>().WithParam(x => x.CollectionId, CurrentCollection.Id).Navigate();
        }

        public void ItemDetails()
        {
            NavigationService.UriFor<ItemDetailsViewModel>().WithParam(x => x.ItemId, SelectedItem.Id).Navigate();
        }
    }
}
