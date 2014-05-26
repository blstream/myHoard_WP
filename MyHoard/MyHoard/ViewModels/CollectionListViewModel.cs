using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyHoard.ViewModels
{
    public class CollectionListViewModel : ViewModelBase, IHandle<ServerMessage>
    {
        
        private List<Collection> collections;
        private Collection selectedCollection;
        private ConfigurationService configurationService;
        private Visibility isSyncVisible;
        private readonly IEventAggregator eventAggregator;
        private bool isFormAccessible;
        private bool isPlaceholderVisible;
        private Visibility isRegisterVisible;
        private Visibility isLogoutVisible;


        private Visibility isProgressBarVisible;
        private CancellationTokenSource tokenSource;

        public CollectionListViewModel(INavigationService navigationService, CollectionService collectionService, ConfigurationService configurationService, IEventAggregator eventAggregator)
            : base(navigationService, collectionService)
        {
            this.configurationService=configurationService;
            this.eventAggregator = eventAggregator;
        }

        public void Settings()
        {
            NavigationService.UriFor<SettingsViewModel>().Navigate();
        }

        public async void Sync()
        {
            IsFormAccessible = false;
            tokenSource = new CancellationTokenSource();
            SynchronizationService ss = new SynchronizationService();
            await ss.SyncDatabase(tokenSource.Token); 
        }

        public void Handle(ServerMessage message)
        {
            IsFormAccessible = true;
            
            MessageBox.Show(message.Message);
            OnActivate();
        }

        public void AddCollection()
        {
            NavigationService.UriFor<AddCollectionViewModel>().Navigate();
        }

        public List<Collection> Collections
        {
            get { return collections; }
            set
            {
                collections = value;
                NotifyOfPropertyChange(() => Collections);
            }
        }

        public Visibility IsSyncVisible
        {
            get { return isSyncVisible; }
            set 
            { 
                isSyncVisible = value;
                NotifyOfPropertyChange(() => IsSyncVisible);
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
            

        public Collection SelectedCollection
        {
            get { return selectedCollection; }
            set 
            { 
                selectedCollection = value;
                NotifyOfPropertyChange(() => SelectedCollection);
            }
        }

        public void CollectionDetails()
        {
            NavigationService.UriFor<CollectionDetailsViewModel>().WithParam(x => x.CollectionId, SelectedCollection.Id).Navigate();
        }

        public void Search()
        {
            NavigationService.UriFor<SearchViewModel>().Navigate();
        }

        public void TakePicture()
        {
            NavigationService.UriFor<CollectionChooserViewModel>().WithParam(x => x.GoToCamera, true).Navigate();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            eventAggregator.Subscribe(this);
            Collections = CollectionService.CollectionList(false, true).OrderBy(e => e.Name).ToList<Collection>();
            IsPlaceholderVisible = Collections.Count < 1;
            if (configurationService.Configuration.IsLoggedIn)
                IsSyncVisible = Visibility.Visible;
            else
                IsSyncVisible = Visibility.Collapsed;
            IsFormAccessible = true;
            if (configurationService.Configuration.IsLoggedIn)
            {
                IsLogoutVisible = Visibility.Visible;
                IsRegisterVisible = Visibility.Collapsed;
            }
            else
            {
                IsRegisterVisible = Visibility.Visible;
                IsLogoutVisible = Visibility.Collapsed;
            }
        }

        public void Register()
        {
            NavigationService.UriFor<RegisterViewModel>().Navigate();
        }

        public void Login()
        {
            NavigationService.UriFor<LoginViewModel>().Navigate();
        }

        public void Logout()
        {
            configurationService.Logout(true);
            OnActivate();
        }

        public Visibility IsRegisterVisible
        {
            get { return isRegisterVisible; }
            set
            {
                isRegisterVisible = value;
                NotifyOfPropertyChange(() => IsRegisterVisible);
            }
        }

        public Visibility IsLogoutVisible
        {
            get { return isLogoutVisible; }
            set
            {
                isLogoutVisible = value;
                NotifyOfPropertyChange(() => IsLogoutVisible);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
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

        
    }
}
