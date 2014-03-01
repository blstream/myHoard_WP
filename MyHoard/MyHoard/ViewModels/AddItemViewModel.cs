using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Resources;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyHoard.ViewModels
{
    public class AddItemViewModel : ViewModelBase, IHandle<ServiceErrorMessage>
    {
        private ItemService itemService;
        private readonly IEventAggregator eventAggregator;

        private string pageTitle;
        private int collectionId;
        private int itemId;
        private Item currentItem;
        private Item editedItem;
        private bool canSave;
        private Visibility isDeleteVisible;


        public AddItemViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService,  IEventAggregator eventAggregator)
            : base(navigationService, collectionService)

        {
            this.itemService = itemService;
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public void DataChanged()
        {
            CanSave = !String.IsNullOrEmpty(CurrentItem.Name) && CurrentItem.Name.Length>=2 && (ItemId == 0 ||
                !StringsEqual(editedItem.Name, CurrentItem.Name) || !StringsEqual(editedItem.Description, CurrentItem.Description));
        }

        public void Save()
        {
            if (ItemId > 0)
            {
                if (itemService.ModifyItem(CurrentItem).Id == CurrentItem.Id)
                {
                    NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, CollectionId).Navigate();
                    this.NavigationService.RemoveBackEntry();
                    this.NavigationService.RemoveBackEntry();
                }
            }
            else
            {
                if (itemService.AddItem(CurrentItem).Id > 0)
                {
                    NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, CollectionId).Navigate();
                    this.NavigationService.RemoveBackEntry();
                    this.NavigationService.RemoveBackEntry();
                }
            }
        }

        public void Delete()
        {
            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteDialog + " \"" + CurrentItem.Name + "\"?", AppResources.Delete, MessageBoxButton.OKCancel);
            if (messageResult == MessageBoxResult.OK)
            {
                itemService.DeleteItem(CurrentItem);
                NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, CollectionId).Navigate();
                this.NavigationService.RemoveBackEntry();
                this.NavigationService.RemoveBackEntry();
            }
        }

        protected override void OnInitialize()
        {
            if (ItemId > 0)
            {
                PageTitle = AppResources.EditItem;
                CurrentItem = itemService.GetItem(ItemId);
                CollectionId = CurrentItem.CollectionId;
                editedItem = new Item()
                {
                    Name = CurrentItem.Name,
                    Description = CurrentItem.Description,
                };
                
                IsDeleteVisible = Visibility.Visible;
            }
            else
            {
                PageTitle = AppResources.AddItem;
                CurrentItem = new Item() { CollectionId = CollectionId };
                IsDeleteVisible = Visibility.Collapsed;
            }
        }

        public void Handle(ServiceErrorMessage message)
        {
            MessageBox.Show(message.Content);
        }

        public bool CanSave
        {
            get { return canSave; }
            set 
            { 
                canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public Visibility IsDeleteVisible
        {
            get { return isDeleteVisible; }
            set
            {
                isDeleteVisible = value;
                NotifyOfPropertyChange(() => IsDeleteVisible);
            }
        }

        public Item EditedItem
        {
            get { return editedItem; }
            set 
            { 
                editedItem = value;
                NotifyOfPropertyChange(() => EditedItem);
            }
        }
                
        public Item CurrentItem
        {
            get { return currentItem; }
            set 
            { 
                currentItem = value;
                NotifyOfPropertyChange(() => CurrentItem);
            }
        }

        public string PageTitle
        {
            get { return pageTitle; }
            set 
            { 
                pageTitle = value;
                NotifyOfPropertyChange(() => PageTitle);
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

        public int ItemId
        {
            get { return itemId; }
            set
            {
                itemId = value;
                NotifyOfPropertyChange(() => ItemId);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        protected override void OnActivate()
        {
            eventAggregator.Subscribe(this);
            base.OnActivate();
        }

        private bool StringsEqual(string string1, string string2)
        {
            return (string.IsNullOrEmpty(string1) && string.IsNullOrEmpty(string2)) ||
                string1 == string2;
        }
    
    }
}
