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
        private Collection currentCollection;
        private ObservableCollection<string> thumbnails;
        private bool canSave;
        private readonly IEventAggregator eventAggregator;

        public AddCollectionViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator)
            : base(navigationService, collectionService)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            PageTitle = AppResources.AddCollection;
            Thumbnails = new BindableCollection<string> { "","\uE114", "\uE104", "\uE107", "\uE10F", "\uE113", "\uE116", "\uE119", "\uE128", "\uE13D", "\uE15D", "\uE15E" };
            CurrentCollection = new Collection();
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

        public ObservableCollection<string> Thumbnails
        {
            get { return thumbnails; }
            set
            {
                thumbnails = value;
                NotifyOfPropertyChange(() => Thumbnails);
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

        public void NameChanged()
        {
            CanSave = !String.IsNullOrEmpty(CurrentCollection.Name); 
        }

        public void Save()
        {
            if (CollectionService.AddCollection(CurrentCollection).Id > 0)
            {
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
    }
}
