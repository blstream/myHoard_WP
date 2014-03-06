using Caliburn.Micro;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.ViewModels
{
    public class ViewModelBase : Screen
    {
        protected INavigationService NavigationService;
        protected CollectionService CollectionService;

        public ViewModelBase(INavigationService navigationService, CollectionService collectionService)
        {
            this.NavigationService = navigationService;
            this.CollectionService = collectionService;
        }

    }
}
