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
        private ObservableCollection<BitmapImage> pictures;

        public ItemDetailsViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService, MediaService mediaService)
            : base(navigationService, collectionService)

        {
            this.mediaService = mediaService;
            this.itemService = itemService;
        }

        protected override void OnInitialize()
        {
            if (ItemId > 0)
            {
                CurrentItem = itemService.GetItem(ItemId);
                Pictures = new ObservableCollection<BitmapImage>(mediaService.PictureDictionary(ItemId).Values.ToList());
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



        public ObservableCollection<BitmapImage> Pictures
        {
            get { return pictures; }
            set
            {
                pictures = value;
                NotifyOfPropertyChange(() => Pictures);
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
