using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Resources;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MyHoard.ViewModels
{
    public class ItemDetailsViewModel : ViewModelBase
    {
        private ItemService itemService;
        private MediaService mediaService;
        private int itemId;
        private Item currentItem;
        private List<Media> pictures;
        private Media selectedPicture;
        private string title;
        private bool isDescriptionVisible;

        public ItemDetailsViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService, MediaService mediaService)
            : base(navigationService, collectionService)

        {
            this.mediaService = mediaService;
            this.itemService = itemService;
        }

        protected override void OnActivate()
        {
            if (ItemId > 0)
            {
                CurrentItem = itemService.GetItem(ItemId);
                Title = CurrentItem.Name;
                Pictures = mediaService.MediaList(ItemId, true, true);
                IsDescriptionVisible = !string.IsNullOrWhiteSpace(CurrentItem.Description);
            }
            
        }


        public void Delete()
        {
            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteDialog + " \"" + CurrentItem.Name + "\"?", AppResources.Delete, MessageBoxButton.OKCancel);
            if (messageResult == MessageBoxResult.OK)
            {
                itemService.DeleteItem(CurrentItem);
                NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, CurrentItem.CollectionId).Navigate();
                this.NavigationService.RemoveBackEntry();
                this.NavigationService.RemoveBackEntry();
            }
        }


        public void Edit()
        {
            NavigationService.UriFor<AddItemViewModel>().WithParam(x => x.ItemId, CurrentItem.Id).Navigate();
        }

        public void ShowPicture()
        {
            NavigationService.UriFor<PictureViewModel>().WithParam(x => x.PictureName, SelectedPicture.FileName).Navigate();
        }

        public Media SelectedPicture
        {
            get { return selectedPicture; }
            set
            {
                selectedPicture = value;
                NotifyOfPropertyChange(() => SelectedPicture);
            }
        }

        public List<Media> Pictures
        {
            get { return pictures; }
            set
            {
                pictures = value;
                NotifyOfPropertyChange(() => Pictures);
            }
        }

        public bool IsDescriptionVisible
        {
            get { return isDescriptionVisible; }
            set
            {
                isDescriptionVisible = value;
                NotifyOfPropertyChange(() => IsDescriptionVisible);
            }
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

        public Item CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                NotifyOfPropertyChange(() => CurrentItem);
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
    }
}
