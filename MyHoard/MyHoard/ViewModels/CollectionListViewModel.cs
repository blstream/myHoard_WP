using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Sync()
        {
            SynchronizationService ss = new SynchronizationService();
            ss.PushCollections(new System.Threading.CancellationToken());
        }

        public void Handle(ServerMessage message)
        {
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

        protected override void OnActivate()
        {
            base.OnActivate();
            eventAggregator.Subscribe(this);
            Collections = CollectionService.CollectionList().OrderBy(e => e.Name).ToList<Collection>();
            if (configurationService.Configuration.IsLoggedIn)
                IsSyncVisible = Visibility.Visible;
            else
                IsSyncVisible = Visibility.Collapsed;
        }

        protected override void OnDeactivate(bool close)
        {
            eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }


        
    }
}
