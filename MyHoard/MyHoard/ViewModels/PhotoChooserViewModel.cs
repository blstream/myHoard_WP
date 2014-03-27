using Caliburn.Micro;
using Microsoft.Xna.Framework.Media;
using MyHoard.Models;
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
    public class PhotoChooserViewModel : ViewModelBase
    {

        private ObservableCollection<Media> pictures;
        private Media selectedPicture;
        private readonly IEventAggregator eventAggregator;

        public PhotoChooserViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator)
            : base(navigationService, collectionService)
        {
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Pictures = new ObservableCollection<Media>();
            MediaLibrary mediaLibrary = new MediaLibrary();

            var spictures = mediaLibrary.Pictures;

            foreach (var picture in spictures)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    addPicture(picture);
                });
            }


        }

        private void addPicture(Picture picture)
        {
            if (picture.Name.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                        picture.Name.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase) ||
                        picture.Name.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase))
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(picture.GetThumbnail());
                WriteableBitmap image = new WriteableBitmap(bi);
                Pictures.Add(new Media() { FileName = picture.Name, Image = image });

            }
        }

        public void GetPicture()
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            var picture = mediaLibrary.Pictures.FirstOrDefault(p => p.Name.Contains(SelectedPicture.FileName));

            if (picture != null)
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(picture.GetImage());
                SelectedPicture.Image = new WriteableBitmap(bi);
            }

            NavigationService.GoBack();
            eventAggregator.Publish(SelectedPicture);
        }

        public Media SelectedPicture
        {
            get { return selectedPicture; }
            set
            {
                selectedPicture = value;
                NotifyOfPropertyChange(() => SelectedPicture);
            }
        }


        public ObservableCollection<Media> Pictures
        {
            get { return pictures; }
            set
            {
                pictures = value;
                NotifyOfPropertyChange(() => Pictures);
            }
        }
    }
}
