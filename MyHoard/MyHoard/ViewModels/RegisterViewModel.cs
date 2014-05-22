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
using System.Windows.Threading;

namespace MyHoard.ViewModels
{
    public class RegisterViewModel : ViewModelBase, IHandle<ServerMessage>
    {

        private readonly IEventAggregator eventAggregator;
        private Dictionary<string, string> backends;
        private bool canRegister;
        private bool isFormAccessible;
        private bool termsAccepted;
        private Visibility isProgressBarVisible;
        private string email;
        private string selectedBackend;
        private PasswordBox passwordBox;
        private RestRequestAsyncHandle asyncHandle;
        private string password;
        private double passwordBoxOpacity;
        private double watermarkOpacity = 100;

        public RegisterViewModel(INavigationService navigationService, CollectionService collectionService, IEventAggregator eventAggregator)
            : base(navigationService, collectionService)
        {
            Backends = ConfigurationService.Backends;
            SelectedBackend = Backends.Keys.First();
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            IsFormAccessible = true;
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
                    if(asyncHandle!=null)
                        asyncHandle.Abort();
                }
            }
        }

        public async void Handle(ServerMessage message)
        {
            IsFormAccessible = true;
            CanRegister = true;
            MessageBox.Show(message.Message);
            
            if (message.IsSuccessfull)
            {
                await IoC.Get<ConfigurationService>().ChangeUser(true);
                NavigationService.UriFor<CollectionListViewModel>().Navigate();
                while (NavigationService.BackStack.Any())
                {
                    this.NavigationService.RemoveBackEntry();
                }
            }
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
            passwordBox = ((RegisterView)view).Password;
            passwordBox.PasswordChanged += new RoutedEventHandler(PasswordChanged);
            base.OnViewLoaded(view);
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = passwordBox.Password;
            DataChanged();

        }
        
        public void DataChanged()
        {
            CanRegister = (!String.IsNullOrEmpty(Email) &&
                Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$") &&
                !String.IsNullOrEmpty(passwordBox.Password) &&
                passwordBox.Password.Length >= 4);
        }

        public void Register()
        {
            IsFormAccessible = false;
            NetworkInformationUtility.GetNetworkTypeAsync(3000);
            
        }

        private void GetNetworkTypeCompleted(object sender, NetworkTypeEventArgs networkTypeEventArgs)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (networkTypeEventArgs.HasInternet)
                {
                    RegistrationService registrationService = new RegistrationService();
                    asyncHandle = registrationService.Register(Email, passwordBox.Password, SelectedBackend);
                }
                else
                {
                    IsFormAccessible = true;
                    CanRegister = true;
                    MessageBox.Show(Resources.AppResources.InternetConnectionError);
                }
            });
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
        
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                NotifyOfPropertyChange(() => Password);
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

        public bool CanRegister
        {
            get { return canRegister; }
            set
            {
                canRegister = value;
                NotifyOfPropertyChange(() => CanRegister);
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
                    CanRegister = false;
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
