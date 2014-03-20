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
    public class CollectionListViewModel : ViewModelBase
    {
        
        private List<Collection> collections;
        private Collection selectedCollection;
        private ConfigurationService configurationService;
        private Visibility isSyncVisible;

            
        public CollectionListViewModel(INavigationService navigationService, CollectionService collectionService, ConfigurationService configurationService)
            : base(navigationService, collectionService)
        {
            this.configurationService=configurationService;
        }

        public void Settings()
        {
            NavigationService.UriFor<SettingsViewModel>().Navigate();
        }

        public void Sync()
        {

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
            Collections = CollectionService.CollectionList().OrderBy(e => e.Name).ToList<Collection>();
            if (configurationService.Configuration.IsLoggedIn)
                IsSyncVisible = Visibility.Visible;
            else
                IsSyncVisible = Visibility.Collapsed;
        }
        
    }
}
