using Caliburn.Micro;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.ViewModels
{
    public class WelcomeViewModel: ViewModelBase
    {
        private readonly ConfigurationService configurationService;

        public WelcomeViewModel(INavigationService navigationService, CollectionService collectionService, ConfigurationService configurationService)
            : base(navigationService, collectionService)
        {
            this.configurationService = configurationService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (configurationService.Configuration.IsLoggedIn)
            {
                NavigationService.UriFor<CollectionListViewModel>().Navigate();
                NavigationService.RemoveBackEntry();
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

        public void UseWithoutAccount()
        {
            NavigationService.UriFor<CollectionListViewModel>().Navigate();
        }

    }
}
