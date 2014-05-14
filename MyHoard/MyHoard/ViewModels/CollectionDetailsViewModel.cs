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
        private int collectionId;
        private Item selectedItem;
        private List<Item> items;
        private bool isPlaceholderVisible;
        private bool isTagsPlaceholderVisible;
        private bool areTagsVisible;
        private string title;
                
        public CollectionDetailsViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService) : base(navigationService,collectionService)
        {
            this.itemService = itemService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            CurrentCollection = CollectionService.GetCollection(CollectionId);
            Items = itemService.ItemList(CollectionId, false, true);
            IsPlaceholderVisible = Items.Count == 0;
            AreTagsVisible = !string.IsNullOrWhiteSpace(CurrentCollection.Tags);
            IsTagsPlaceholderVisible = !AreTagsVisible && string.IsNullOrWhiteSpace(CurrentCollection.Description);
            Title = CurrentCollection.Name;
        }

        public void SortAlphabetically()
        {
            Items = Items.OrderBy(x => x.Name).ToList();
        }

        public void SortAlphabeticallyZA()
        {
            Items = Items.OrderByDescending(x => x.Name).ToList();
        }

        public void SortFromNewest()
        {
            Items = Items.OrderBy(x => x.CreatedDate).ToList();
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
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

        public bool IsPlaceholderVisible
        {
            get { return isPlaceholderVisible; }
            set
            {
                isPlaceholderVisible = value;
                NotifyOfPropertyChange(() => IsPlaceholderVisible);
            }
        }

        public bool IsTagsPlaceholderVisible
        {
            get { return isTagsPlaceholderVisible; }
            set
            {
                isTagsPlaceholderVisible = value;
                NotifyOfPropertyChange(() => IsTagsPlaceholderVisible);
            }
        }

        public bool AreTagsVisible
        {
            get { return areTagsVisible; }
            set
            {
                areTagsVisible = value;
                NotifyOfPropertyChange(() => AreTagsVisible);
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
