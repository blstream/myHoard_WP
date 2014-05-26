using Caliburn.Micro;
using Microsoft.Phone.Tasks;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyHoard.ViewModels
{
    public class CollectionChooserViewModel: ViewModelBase, IHandle<TaskCompleted<PhotoResult>>, IHandle<ViewModelLoadedMessage>
    {

        private bool goToCamera = false;
        private bool firstEntry = true;
        private Collection selectedCollection;
        private List<Collection> collections;
        private Media photo;
        private readonly IEventAggregator eventAggregator;
        public CollectionChooserViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator)
            : base(navigationService, collectionService)
        {
            this.eventAggregator = eventAggregator;
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

        public List<Collection> Collections
        {
            get { return collections; }
            set
            {
                collections = value;
                NotifyOfPropertyChange(() => Collections);
            }

        }

        public void CollectionChosen()
        {
            if (SelectedCollection != null)
            {
                staySubscribed = true;
                NavigationService.UriFor<AddItemViewModel>().WithParam(x => x.CollectionId, SelectedCollection.Id).WithParam(x=>x.InvertedFlow,true).Navigate();
            }
        }

        public bool GoToCamera
        {
            get { return goToCamera; }
            set
            {
                goToCamera = value;
                NotifyOfPropertyChange(() => GoToCamera);
            }
        }

        private bool staySubscribed;

        protected override void OnActivate()
        {
            base.OnActivate();
            eventAggregator.Subscribe(this);
            if (GoToCamera && firstEntry)
            {
                staySubscribed = true;
                GoToCamera = false;
                firstEntry = false;
                eventAggregator.RequestTask<CameraCaptureTask>();
            }
            else
            {
                staySubscribed = false;
                Collections = CollectionService.CollectionList(false, false);
            }
        }

        public void Handle(ViewModelLoadedMessage m)
        {
            eventAggregator.Publish(photo);
            eventAggregator.Unsubscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            if (!staySubscribed)
                eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public void Handle(TaskCompleted<PhotoResult> e)
        {
            if (e.Result.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(e.Result.ChosenPhoto);
                WriteableBitmap image = new WriteableBitmap(bi);
                photo = new Media() { Image = image }; 
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        
    }
}
