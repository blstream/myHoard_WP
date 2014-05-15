using Caliburn.Micro;
using Microsoft.Phone.Tasks;
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
    public class AddItemViewModel : ViewModelBase, IHandle<ServiceErrorMessage>, IHandle<TaskCompleted<PhotoResult>>, IHandle<Media>
    {
        private ItemService itemService;
        private MediaService mediaService;
        private readonly IEventAggregator eventAggregator;
        private string pageTitle;
        private int collectionId;
        private int itemId;
        private Item currentItem;
        private Item editedItem;
        private bool canSave;
        private bool staySubscribed;
        private bool newItem;
        private Media selectedPicture;

        private ObservableCollection<Media> pictures;
        private List<Media> picturesToDelete;
        

    
        public AddItemViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService,  IEventAggregator eventAggregator, MediaService mediaService)
            : base(navigationService, collectionService)

        {
            this.mediaService = mediaService;
            this.itemService = itemService;
            this.eventAggregator = eventAggregator;
        }

        public void DeleteImage()
        {
            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteDialog + " " + AppResources.ThisImage + "?", AppResources.Delete, MessageBoxButton.OKCancel);
            if (messageResult == MessageBoxResult.OK)
            {
                if (!String.IsNullOrEmpty(SelectedPicture.FileName))
                {
                    SelectedPicture.ToDelete = true;
                    picturesToDelete.Add(SelectedPicture);
                }
                Pictures.Remove(SelectedPicture);
                DataChanged();
            }
        }

        public void GetGeolcation()
        {
            if (CurrentItem.LocationSet)
                GeolocationHelper.GetCurrentLocation(CurrentItem);
                //    .ContinueWith(_ => GeolocationHelper.GetLocationName(CurrentItem));
            else
            {
                GeolocationHelper.ClearLocation(CurrentItem);
            }
        }

        public void Trim()
        {
            if (!string.IsNullOrEmpty(CurrentItem.Name))
                CurrentItem.Name = CurrentItem.Name.Trim();
            if (!string.IsNullOrEmpty(CurrentItem.Description))
                CurrentItem.Description = CurrentItem.Description.Trim();
        }

        public void DataChanged()
        {
            bool picturesChanged=false;
           
            if (picturesToDelete.Count>0)
            {
                picturesChanged = true;
            }
            else
                foreach (Media m in Pictures)
                {
                    if (String.IsNullOrEmpty(m.FileName))
                    {
                        picturesChanged = true;
                    }
                }

            CanSave = !String.IsNullOrEmpty(CurrentItem.Name) && CurrentItem.Name.Length>=2 && (ItemId == 0 ||
                !StringsEqual(editedItem.Name, CurrentItem.Name) || !StringsEqual(editedItem.Description, CurrentItem.Description) || picturesChanged ||
                editedItem.LocationLat != CurrentItem.LocationLat || editedItem.LocationLng != CurrentItem.LocationLng || editedItem.LocationSet != CurrentItem.LocationSet);
        }


        public void TakePicture()
        {
            eventAggregator.RequestTask<CameraCaptureTask>();
        }

        public void TakePictureFromGallery()
        {
            staySubscribed = true;
            NavigationService.UriFor<PhotoChooserViewModel>().Navigate();
        }

        public void Handle(Media m)
        {
            staySubscribed = false;
            m.FileName = "";
            m.ItemId = ItemId;
            Pictures.Add(m);
            DataChanged();
        }

        public void Handle(TaskCompleted<PhotoResult> e)
        {
            if(e.Result.TaskResult==Microsoft.Phone.Tasks.TaskResult.OK)
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(e.Result.ChosenPhoto);
                WriteableBitmap image = new WriteableBitmap(bi);
                Pictures.Add(new Media() { ItemId = itemId, Image = image });
                DataChanged();
            }
        }

        
        public void Save()
        {
            Trim();
            if (NewItem)
            {
                if (itemService.AddItem(CurrentItem).Id > 0)
                {
                    foreach(Media m in Pictures)
                    {
                        m.ItemId = CurrentItem.Id;
                    }
                    mediaService.SavePictureList(Pictures);
                    NavigationService.UriFor<ItemDetailsViewModel>().WithParam(x => x.ItemId, CurrentItem.Id).Navigate();
                    this.NavigationService.RemoveBackEntry();
                }
            }
            else
            {
                if (itemService.ModifyItem(CurrentItem).Id == CurrentItem.Id)
                {
                    mediaService.SavePictureList(Pictures);
                    mediaService.SavePictureList(picturesToDelete);
                    NavigationService.GoBack();
                }
            }
        }

        protected override void OnInitialize()
        {
            NewItem = ItemId == 0;
            if (NewItem)
            {   PageTitle = AppResources.NewElement;
                CurrentItem = new Item() { CollectionId = CollectionId };
                Pictures = new ObservableCollection<Media>();
            }
            else
            {
                PageTitle = AppResources.EditElement;
                CurrentItem = itemService.GetItem(ItemId);
                CollectionId = CurrentItem.CollectionId;
                editedItem = new Item()
                {
                    Name = CurrentItem.Name,
                    Description = CurrentItem.Description,
                };
                Pictures = new ObservableCollection<Media>(mediaService.MediaList(ItemId, true, true));
				
                if (CurrentItem.LocationSet)
                    GeolocationHelper.GetLocationName(CurrentItem);
            }

            picturesToDelete = new List<Media>();
        }

        public void Handle(ServiceErrorMessage message)
        {
            MessageBox.Show(message.Content);
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

        public bool CanSave
        {
            get { return canSave; }
            set 
            { 
                canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public ObservableCollection<Media> Pictures
        {
            get { return pictures; }
            set
            {
                pictures = value;
                NotifyOfPropertyChange(() => Pictures);
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

        public bool NewItem
        {
            get { return newItem; }
            set
            {
                newItem= value;
                NotifyOfPropertyChange(()=>NewItem);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            if(!staySubscribed)
                eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        protected override void OnActivate()
        {
            if(!staySubscribed)
                eventAggregator.Subscribe(this);
            base.OnActivate();
            staySubscribed = false;
        }

        private bool StringsEqual(string string1, string string2)
        {
            return (string.IsNullOrEmpty(string1) && string.IsNullOrEmpty(string2)) ||
                string1 == string2;
        }
    
    }
}
