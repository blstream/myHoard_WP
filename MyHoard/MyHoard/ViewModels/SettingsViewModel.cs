using Caliburn.Micro;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyHoard.ViewModels
{
    public class SettingsViewModel:ViewModelBase
    {
        private Visibility isRegisterVisible;
        private Visibility isLogoutVisible;
        private ConfigurationService configurationService;

        public SettingsViewModel(INavigationService navigationService, CollectionService collectionService, ConfigurationService configurationService)
            : base(navigationService, collectionService)
        {
            this.configurationService = configurationService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if(configurationService.Configuration.IsLoggedIn)
            {
                IsLogoutVisible = Visibility.Visible;
                IsRegisterVisible = Visibility.Collapsed;
            }
            else
            {
                IsRegisterVisible = Visibility.Visible;
                IsLogoutVisible = Visibility.Collapsed;
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

        public void Logout()
        {
            configurationService.Logout();
            NavigationService.GoBack();
        }

        public Visibility IsRegisterVisible
        {
            get { return isRegisterVisible; }
            set
            {
                isRegisterVisible = value;
                NotifyOfPropertyChange(() => IsRegisterVisible);
            }
        }

        public Visibility IsLogoutVisible
        {
            get { return isLogoutVisible; }
            set
            {
                isLogoutVisible = value;
                NotifyOfPropertyChange(() => IsLogoutVisible);
            }
        }
    }
}
