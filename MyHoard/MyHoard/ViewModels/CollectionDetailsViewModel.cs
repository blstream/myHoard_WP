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
    public class CollectionDetailsViewModel : ViewModelBase
    {
        private Collection currentCollection;
        private String collectionName;
        private int collectionId;
                
        
        public CollectionDetailsViewModel(INavigationService navigationService, CollectionService collectionService) : base(navigationService,collectionService)
        {

        }

        public Collection CurrentCollection
        {
            get { return currentCollection; }
            set 
            { 
                currentCollection = value;
                NotifyOfPropertyChange(() => CurrentCollection);
                CollectionName = CurrentCollection.Name;
            }
        }

        public String CollectionName
        {
            get { return collectionName; }
            set 
            { 
                collectionName = value;
                NotifyOfPropertyChange(() => CollectionName);
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

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CurrentCollection = CollectionService.GetCollection(CollectionId);
        }

        public void Edit()
        {
            NavigationService.UriFor<AddCollectionViewModel>().WithParam(x => x.CollectionId, CurrentCollection.Id).Navigate();
        }

    }
}
