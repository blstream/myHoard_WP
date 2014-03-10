using Caliburn.Micro;
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
    public class PictureViewModel : ViewModelBase
    {
        private MediaService mediaService;
        private BitmapImage picture;
        private string pictureName;

        public PictureViewModel(INavigationService navigationService, CollectionService collectionService, MediaService mediaService)
            : base(navigationService, collectionService)
        {
            this.mediaService = mediaService;
        }

        protected override void OnInitialize()
        {
            Picture = mediaService.GetPictureFromIsolatedStorage(new Media() { FileName = PictureName });
        }

        public BitmapImage Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                NotifyOfPropertyChange(() => Picture);
            }
        }

        public string PictureName
        {
            get { return pictureName; }
            set
            {
                pictureName = value;
                NotifyOfPropertyChange(() => PictureName);
            }
        }
    }
}
