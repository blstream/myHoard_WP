using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Resources;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyHoard.ViewModels
{
    public class AddCollectionViewModel : ViewModelBase, IHandle<ServiceErrorMessage>, IHandle<ServerMessage>
    {
        private string pageTitle;
        private string selectedTag;
        private string newTag;
        private Collection currentCollection;
        private Collection editedCollection;
        private bool canSave;
        private bool canAddTag;
        private bool isLoggedIn;
        private Visibility isDeleteVisible;
        private int collectionId;
        private readonly IEventAggregator eventAggregator;
        private ObservableCollection<string> tags;
        private ConfigurationService configurationService;
        private Visibility isProgressBarVisible;
        private bool isFormAccessible;
        private CancellationTokenSource tokenSource;

               
        public AddCollectionViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator, ConfigurationService configurationService)
            : base(navigationService, collectionService)
        {
            this.eventAggregator = eventAggregator;
            this.configurationService = configurationService;
            eventAggregator.Subscribe(this);
        }

        public void OnGoBack(CancelEventArgs eventArgs)
        {
            if (!IsFormAccessible)
            {
                MessageBoxResult messageResult = MessageBox.Show(Resources.AppResources.CancelConfirm, "", MessageBoxButton.OKCancel);
                if (messageResult == MessageBoxResult.OK)
                {
                    tokenSource.Cancel();
                    IsFormAccessible = true;
                    CollectionService.ModifyCollection(editedCollection);
                }
                eventArgs.Cancel = true;
            }
        }

        public bool IsFormAccessible
        {
            get { return isFormAccessible; }
            set
            {
                isFormAccessible = value;
                NotifyOfPropertyChange(() => IsFormAccessible);
                IsProgressBarVisible = (IsFormAccessible ? Visibility.Collapsed : Visibility.Visible);
            }
        }

        public Visibility IsProgressBarVisible
        {
            get { return isProgressBarVisible; }
            set
            {
                isProgressBarVisible = value;
                NotifyOfPropertyChange(() => IsProgressBarVisible);
            }
        }

        public ObservableCollection<string> Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                NotifyOfPropertyChange(() => Tags);
            }
        }

        public string SelectedTag
        {
            get { return selectedTag; }
            set
            {
                selectedTag = value;
                NotifyOfPropertyChange(() => SelectedTag);
            }
        }

        public string NewTag
        {
            get { return newTag; }
            set
            {
                newTag = value;
                NotifyOfPropertyChange(() => NewTag);
                if (String.IsNullOrEmpty(value))
                    CanAddTag = false;
                else
                    CanAddTag = true;
            }
        }

        public void AddTag()
        {
            if (!Tags.Contains(NewTag))
            {
                Tags.Add(NewTag);
                CurrentCollection.TagList = Tags.ToList();
                DataChanged();
            }
            NewTag = "";
        }

        public void DeleteTag()
        {
            Tags.Remove(SelectedTag);
            CurrentCollection.TagList = Tags.ToList();
            DataChanged();
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


                
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                NotifyOfPropertyChange(() => IsLoggedIn);
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

        public bool CanAddTag
        {
            get { return canAddTag; }
            set
            {
                canAddTag = value;
                NotifyOfPropertyChange(() => CanAddTag);
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

        public void Handle(ServerMessage message)
        {
            if (message.IsSuccessfull)
            {
                if (CurrentCollection.IsPrivate)
                {
                    MessageBox.Show(AppResources.DeleteFromServerSuccess);
                }
            }
            else
            {
                MessageBox.Show(message.Message + "\n" + AppResources.SyncError);
            }
            NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, CurrentCollection.Id).Navigate();
            this.NavigationService.RemoveBackEntry();
            this.NavigationService.RemoveBackEntry();
        }

        public void DataChanged()
        {
            CanSave = !String.IsNullOrEmpty(CurrentCollection.Name) && (CollectionId==0 || 
                !StringsEqual(editedCollection.Name, CurrentCollection.Name) || !StringsEqual(editedCollection.Description,CurrentCollection.Description)
                || !StringsEqual(editedCollection.Tags, CurrentCollection.Tags) || editedCollection.IsPrivate != CurrentCollection.IsPrivate);
        }

        public async void Save()
        {
            if (CollectionId > 0)
            {
                if (CollectionService.ModifyCollection(CurrentCollection).Id == CurrentCollection.Id)
                {
                    if (CurrentCollection.IsPrivate != editedCollection.IsPrivate)
                    {
                        IsFormAccessible = false;
                        tokenSource = new CancellationTokenSource();
                        SynchronizationService synchronizationService = new SynchronizationService();
                        if (CurrentCollection.IsPrivate)
                        {
                            MessageBoxResult messageResult = MessageBox.Show(AppResources.DeleteFromServerDialog, AppResources.Delete, MessageBoxButton.OKCancel);
                            if (messageResult == MessageBoxResult.OK)
                            {
                                await synchronizationService.SyncDatabase(tokenSource.Token);
                            }
                            else
                            {
                                CollectionService.ModifyCollection(editedCollection);
                            }
                        }
                        else
                        {
                            await synchronizationService.SyncDatabase(tokenSource.Token); 
                        }
                        return;
                    }

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

        public void Handle(ServiceErrorMessage message)
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
            IsFormAccessible = true;
            eventAggregator.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnInitialize()
        {
            IsLoggedIn = configurationService.Configuration.IsLoggedIn;
            if (CollectionId > 0)
            {
                PageTitle = AppResources.EditCollection;
                CurrentCollection = CollectionService.GetCollection(CollectionId);
                editedCollection = new Collection()
                {
                    Name = CurrentCollection.Name,
                    Description = CurrentCollection.Description,
                    CreatedDate = CurrentCollection.CreatedDate,
                    Id = CurrentCollection.Id,
                    ItemsNumber = CurrentCollection.ItemsNumber,
                    ServerId = CurrentCollection.ServerId,
                    IsSynced = CurrentCollection.IsSynced,
                    ModifiedDate = CurrentCollection.ModifiedDate,
                    Tags = CurrentCollection.Tags,
                    IsPrivate = CurrentCollection.IsPrivate
                };
                Tags = new ObservableCollection<string>(CurrentCollection.TagList);
                IsDeleteVisible = Visibility.Visible;
            }
            else
            {
                PageTitle = AppResources.AddCollection;
                CurrentCollection = new Collection() { IsPrivate = true };
                Tags = new ObservableCollection<string>();
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
