using Caliburn.Micro;
using DotNetApp.Utilities;
using MyHoard.Services;
using MyHoard.Views;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyHoard.ViewModels
{
    public class LoginViewModel : ViewModelBase, IHandle<ServerMessage>
    {

        private readonly IEventAggregator eventAggregator;
        private Dictionary<string, string> backends;
        private bool canLogin;
        private bool isFormAccessible;
        private bool keepLogged;
        private bool changeUser;
        private bool copyDefault;
        private Visibility isProgressBarVisible;
        private string email;
        private string selectedBackend;
        private PasswordBox passwordBox;
        private RestRequestAsyncHandle asyncHandle;
        private ConfigurationService configurationService;
        private ItemService itemService;
        private MediaService mediaService;
        private double passwordBoxOpacity;
        private double watermarkOpacity=100;

        public LoginViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator, ConfigurationService configurationService, ItemService itemService, MediaService mediaService)
            : base(navigationService, collectionService)
        {
            Backends = ConfigurationService.Backends;
            SelectedBackend = Backends.Keys.First();
            this.eventAggregator = eventAggregator;
            this.configurationService = configurationService;
            eventAggregator.Subscribe(this);
            IsFormAccessible = true;
            this.mediaService = mediaService;
            this.itemService = itemService;
        }

        public void OnGoBack(CancelEventArgs eventArgs)
        {
            if (!IsFormAccessible)
            {
                MessageBoxResult messageResult = MessageBox.Show(Resources.AppResources.CancelConfirm, "", MessageBoxButton.OKCancel);
                if (messageResult == MessageBoxResult.Cancel)
                {
                    eventArgs.Cancel = true;
                }
                else
                {
                    if (asyncHandle != null)
                        asyncHandle.Abort();
                }
            }
        }


        public async void Handle(ServerMessage message)
        {
            IsFormAccessible = true; ;
            CanLogin = true;

            MessageBox.Show(message.Message);

            if (message.IsSuccessfull)
            {
                if(changeUser)
                {
                    await configurationService.ChangeUser(copyDefault);
                }
                NavigationService.UriFor<CollectionListViewModel>().Navigate();
                while (NavigationService.BackStack.Any())
                {
                    this.NavigationService.RemoveBackEntry();
                }
            }
        }

        private void GetNetworkTypeCompleted(object sender, NetworkTypeEventArgs networkTypeEventArgs)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (networkTypeEventArgs.HasInternet)
                {
                    if (configurationService.Configuration.Email != Email)
                        changeUser = true;
                    if (string.IsNullOrWhiteSpace(configurationService.Configuration.Email))
                        copyDefault = true;
                    RegistrationService registrationService = new RegistrationService();
                    asyncHandle = registrationService.Login(Email, passwordBox.Password, KeepLogged, SelectedBackend);
                }
                else
                {
                    IsFormAccessible = true;
                    CanLogin = true;
                    MessageBox.Show(Resources.AppResources.InternetConnectionError);
                }
            });
        }

        public void Login()
        {
            IsFormAccessible = false;
            NetworkInformationUtility.GetNetworkTypeAsync(3000);
        }


        protected override void OnDeactivate(bool close)
        {
            eventAggregator.Unsubscribe(this);
            NetworkInformationUtility.GetNetworkTypeCompleted -= GetNetworkTypeCompleted;
            base.OnDeactivate(close);
        }


        protected override void OnActivate()
        {
            eventAggregator.Subscribe(this);
            base.OnActivate();
            NetworkInformationUtility.GetNetworkTypeCompleted += GetNetworkTypeCompleted;
        }


        protected override void OnViewLoaded(object view)
        {
            passwordBox = ((LoginView)view).Password;
            passwordBox.PasswordChanged += new RoutedEventHandler(PasswordChanged);
            base.OnViewLoaded(view);
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            DataChanged();
        }

        public void DataChanged()
        {
            CanLogin = (!String.IsNullOrWhiteSpace(Email) && Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$", RegexOptions.IgnoreCase) &&
                !String.IsNullOrWhiteSpace(passwordBox.Password));
        }



        public void PasswordLostFocus()
        {
            CheckPasswordWatermark();
        }

        public void CheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(passwordBox.Password);
            WatermarkOpacity = passwordEmpty ? 100 : 0;
            PasswordBoxOpacity = passwordEmpty ? 0 : 100;
        }

        public void PasswordGotFocus()
        {
            WatermarkOpacity = 0;
            PasswordBoxOpacity = 100;
        }



        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public double PasswordBoxOpacity
        {
            get { return passwordBoxOpacity; }
            set
            {
                passwordBoxOpacity = value;
                NotifyOfPropertyChange(() => PasswordBoxOpacity);
            }
        }

        public double WatermarkOpacity
        {
            get { return watermarkOpacity; }
            set
            {
                watermarkOpacity = value;
                NotifyOfPropertyChange(() => WatermarkOpacity);
            }
        }


        public string SelectedBackend
        {
            get { return selectedBackend; }
            set
            {
                selectedBackend = value;
                NotifyOfPropertyChange(() => SelectedBackend);
            }
        }

        public bool CanLogin
        {
            get { return canLogin; }
            set
            {
                canLogin = value;
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool KeepLogged
        {
            get { return keepLogged; }
            set
            {
                keepLogged = value;
                NotifyOfPropertyChange(() => KeepLogged);
            }
        }

        public bool IsFormAccessible
        {
            get { return isFormAccessible; }
            set
            {
                isFormAccessible = value;
                NotifyOfPropertyChange(() => IsFormAccessible);
                if (!value)
                    CanLogin = false;
                IsProgressBarVisible = (IsFormAccessible ? Visibility.Collapsed : Visibility.Visible);
            }
        }

        public Visibility IsProgressBarVisible
        {
            get { return isProgressBarVisible; }
            set
            {
                isProgressBarVisible = value;
                NotifyOfPropertyChange(() => IsProgressBarVisible);
            }
        }


        public Dictionary<string, string> Backends
        {
            get { return backends; }
            set
            {
                backends = value;
                NotifyOfPropertyChange(() => Backends);
            }
        }

    }
}
