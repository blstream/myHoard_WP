using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Resources;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyHoard.ViewModels
{
    public class CollectionDetailsViewModel : ViewModelBase
    {
        private ItemService itemService;
        private Collection currentCollection;
        private int collectionId;
        private Item selectedItem;
        private ObservableCollection<Item> items;
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
            Items = new ObservableCollection<Item>(itemService.ItemList(CollectionId, false, true));
            SortAlphabetically();
            IsPlaceholderVisible = Items.Count == 0;
            AreTagsVisible = !string.IsNullOrWhiteSpace(CurrentCollection.Tags);
            IsTagsPlaceholderVisible = !AreTagsVisible && string.IsNullOrWhiteSpace(CurrentCollection.Description);
            Title = CurrentCollection.Name;
        }

        
        public void Edit(Item item)
        {
            NavigationService.UriFor<AddItemViewModel>().WithParam(x => x.ItemId, item.Id).Navigate();
        }

        public void Delete(Item item)
        {
            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteDialog + " \"" + item.Name + "\"?", AppResources.Delete, MessageBoxButton.OKCancel);
            if (messageResult == MessageBoxResult.OK)
            {
                Items.Remove(item);
                itemService.DeleteItem(item);
            }
        }

        public void DeleteCollection()
        {
            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteDialog + " \"" + Title + "\"?", AppResources.Delete, MessageBoxButton.OKCancel);
            if (messageResult == MessageBoxResult.OK)
            {
                CollectionService.DeleteCollection(CurrentCollection);
                NavigationService.UriFor<CollectionListViewModel>().Navigate();
                while (NavigationService.BackStack.Any())
                {
                    this.NavigationService.RemoveBackEntry();
                }
            }
        }

        public void SortAlphabetically()
        {
            Items= new ObservableCollection<Item>(Items.OrderBy(x => x.Name));
        }

        public void SortAlphabeticallyZA()
        {
            Items= new ObservableCollection<Item>(Items.OrderByDescending(x => x.Name));
        }

        public void SortFromNewest()
        {
            Items= new ObservableCollection<Item>(Items.OrderByDescending(x => x.CreatedDate));
        }

        public void SortFromOldest()
        {
            Items = new ObservableCollection<Item>(Items.OrderBy(x => x.CreatedDate));
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

        public ObservableCollection<Item> Items
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
                   

        public void EditColection()
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
