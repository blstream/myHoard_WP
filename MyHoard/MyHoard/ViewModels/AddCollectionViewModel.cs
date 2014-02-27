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

namespace MyHoard.ViewModels
{
    public class AddCollectionViewModel : ViewModelBase, IHandle<CollectionServiceErrorMessage>
    {
        private string pageTitle;
        private string thumbnail;
        private Collection currentCollection;
        private Collection editedCollection;
        private ObservableCollection<string> thumbnails;
        private bool canSave;
        private Visibility isDeleteVisible;
        private int collectionId;
        private readonly IEventAggregator eventAggregator;

        public AddCollectionViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator)
            : base(navigationService, collectionService)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            Thumbnails = new BindableCollection<string> { "","\uE114", "\uE104", "\uE107", "\uE10F", "\uE113", "\uE116", "\uE119", "\uE128", "\uE13D", "\uE15D", "\uE15E" };
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


        public ObservableCollection<string> Thumbnails
        {
            get { return thumbnails; }
            set
            {
                thumbnails = value;
                NotifyOfPropertyChange(() => Thumbnails);
            }
        }

        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                CurrentCollection.Thumbnail = value;
                thumbnail = value;
                NotifyOfPropertyChange(() => Thumbnail);
                DataChanged();
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

        public Visibility IsDeleteVisible
        {
            get { return isDeleteVisible; }
            set 
            { 
                isDeleteVisible = value;
                NotifyOfPropertyChange(() => IsDeleteVisible);
            }
        }

        public void DataChanged()
        {
            CanSave = !String.IsNullOrEmpty(CurrentCollection.Name) && (CollectionId==0 || 
                !StringsEqual(editedCollection.Name, CurrentCollection.Name) || !StringsEqual(editedCollection.Description,CurrentCollection.Description)
                || !StringsEqual(editedCollection.Thumbnail, CurrentCollection.Thumbnail));
        }

        public void Save()
        {
            if (CollectionId > 0)
            {
                if (CollectionService.ModifyCollection(CurrentCollection).Id == CurrentCollection.Id)
                {
                    NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, CurrentCollection.Id).Navigate();
                    this.NavigationService.RemoveBackEntry();
                    this.NavigationService.RemoveBackEntry();
                }
            }
            else
            {
                if (CollectionService.AddCollection(CurrentCollection).Id > 0)
                {
                    NavigationService.UriFor<CollectionListViewModel>().Navigate();
                    this.NavigationService.RemoveBackEntry();
                    this.NavigationService.RemoveBackEntry();
                }
            }
        }

        public void Delete()
        {
            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteDialog+" \"" + CurrentCollection.Name +"\"?",AppResources.Delete,MessageBoxButton.OKCancel);
            if(messageResult==MessageBoxResult.OK)
            {
                CollectionService.DeleteCollection(CurrentCollection);
                NavigationService.UriFor<CollectionListViewModel>().Navigate();
                while (NavigationService.BackStack.Any())
                {
                        this.NavigationService.RemoveBackEntry();
                }
            }
        }

        public void Handle(CollectionServiceErrorMessage message)
        {
            MessageBox.Show(message.Content);
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

        protected override void OnInitialize()
        {
            if (CollectionId > 0)
            {
                PageTitle = AppResources.EditCollection;
                CurrentCollection = CollectionService.GetCollection(CollectionId);
                editedCollection = new Collection()
                {
                    Name = CurrentCollection.Name,
                    Description = CurrentCollection.Description,
                    Thumbnail = CurrentCollection.Thumbnail
                };
                Thumbnail = CurrentCollection.Thumbnail;
                IsDeleteVisible = Visibility.Visible;
            }
            else
            {
                PageTitle = AppResources.AddCollection;
                CurrentCollection = new Collection();
                Thumbnail = CurrentCollection.Thumbnail;
                IsDeleteVisible = Visibility.Collapsed;
            }
        }
        private bool StringsEqual(string string1, string string2)
        {
            return (string.IsNullOrEmpty(string1) && string.IsNullOrEmpty(string2)) ||
                string1 == string2;
        }
    }
}
