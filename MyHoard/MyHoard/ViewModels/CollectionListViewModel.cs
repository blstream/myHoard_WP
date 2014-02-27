using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.ViewModels
{
    public class CollectionListViewModel : ViewModelBase
    {
        
        private List<Collection> collections;
        private Collection selectedCollection;
                
        public CollectionListViewModel(INavigationService navigationService, CollectionService collectionService)
            : base(navigationService, collectionService)
        {
            Collections = collectionService.CollectionList().OrderBy(e => e.Name).ToList<Collection>();
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
        
    }
}
