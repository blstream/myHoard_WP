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
    public class AddItemViewModel : ViewModelBase, IHandle<ServiceErrorMessage>, IHandle<TaskCompleted<PhotoResult>>
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
        private Visibility isDeleteVisible;

        private Dictionary<Media,BitmapImage> pictureDictionary;
        private ObservableCollection<BitmapImage> pictures;

        


        public AddItemViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService,  IEventAggregator eventAggregator, MediaService mediaService)
            : base(navigationService, collectionService)

        {
            this.mediaService = mediaService;
            this.itemService = itemService;
            this.eventAggregator = eventAggregator;
        }

        public void DataChanged()
        {
            bool picturesChanged=false;
            foreach (Media m in PictureDictionary.Keys)
            {
                if(String.IsNullOrEmpty(m.FileName))
                {
                    picturesChanged = true;
                }
            }
            CanSave = !String.IsNullOrEmpty(CurrentItem.Name) && CurrentItem.Name.Length>=2 && (ItemId == 0 ||
                !StringsEqual(editedItem.Name, CurrentItem.Name) || !StringsEqual(editedItem.Description, CurrentItem.Description) || picturesChanged);
        }


        public void TakePicture()
        {
            eventAggregator.RequestTask<CameraCaptureTask>();
        }


        public void Handle(TaskCompleted<PhotoResult> e)
        {
            if(e.Result.TaskResult==Microsoft.Phone.Tasks.TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.Result.ChosenPhoto);
                PictureDictionary.Add(new Media() { ItemId = itemId },image);
                Pictures.Add(image);
                DataChanged();
            }
        }

        
        public void Save()
        {
            if (ItemId > 0)
            {
                if (itemService.ModifyItem(CurrentItem).Id == CurrentItem.Id)
                {
                    mediaService.SavePictureDicitonary(PictureDictionary);
                    NavigationService.UriFor<ItemDetailsViewModel>().WithParam(x => x.ItemId, ItemId).Navigate();
                    this.NavigationService.RemoveBackEntry();
                    this.NavigationService.RemoveBackEntry();
                    
                }
            }
            else
            {
                if (itemService.AddItem(CurrentItem).Id > 0)
                {
                    foreach(Media m in PictureDictionary.Keys)
                    {
                        m.ItemId = CurrentItem.Id;
                    }
                    mediaService.SavePictureDicitonary(PictureDictionary);
                    NavigationService.UriFor<ItemDetailsViewModel>().WithParam(x => x.ItemId, CurrentItem.Id).Navigate();
                    this.NavigationService.RemoveBackEntry();
                }
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
                PictureDictionary = mediaService.PictureDictionary(ItemId);
                Pictures = new ObservableCollection<BitmapImage>(PictureDictionary.Values.ToList());
                IsDeleteVisible = Visibility.Visible;
            }
            else
            {
                PageTitle = AppResources.AddItem;
                CurrentItem = new Item() { CollectionId = CollectionId };
                IsDeleteVisible = Visibility.Collapsed;
                PictureDictionary = new Dictionary<Media, BitmapImage>();
                Pictures = new ObservableCollection<BitmapImage>();
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

        public ObservableCollection<BitmapImage> Pictures
        {
            get { return pictures; }
            set
            {
                pictures = value;
                NotifyOfPropertyChange(() => Pictures);
            }
        }

        public Dictionary<Media, BitmapImage> PictureDictionary
        {
            get { return pictureDictionary; }
            set
            {
                pictureDictionary = value;
                NotifyOfPropertyChange(() => PictureDictionary);
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
