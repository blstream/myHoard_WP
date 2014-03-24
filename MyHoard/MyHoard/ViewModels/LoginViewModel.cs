using Caliburn.Micro;
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
        private bool dropTables;
        private Visibility isProgressBarVisible;
        private string userName;
        private string selectedBackend;
        private PasswordBox passwordBox;
        private RestRequestAsyncHandle asyncHandle;
        private ConfigurationService configurationService;
        private ItemService itemService;
        private MediaService mediaService;

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
                    asyncHandle.Abort();
                }
            }
        }


        public void Handle(ServerMessage message)
        {
            IsFormAccessible = true; ;
            CanLogin = true;

            MessageBox.Show(message.Message);

            if (message.IsSuccessfull)
            {
                if(dropTables)
                {
                    CollectionService.DeleteAll();
                    itemService.DeleteAll();
                    mediaService.DeleteAll();
                }
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
            base.OnDeactivate(close);
        }


        protected override void OnActivate()
        {
            eventAggregator.Subscribe(this);
            base.OnActivate();
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
            CanLogin = (!String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(passwordBox.Password));
        }



        public void Login()
        {
            if (!string.IsNullOrWhiteSpace(configurationService.Configuration.UserName) && configurationService.Configuration.UserName != UserName)
                dropTables = true;
            else
                dropTables = false;
            IsFormAccessible = false;
            RegistrationService registrationService = new RegistrationService();
            asyncHandle = registrationService.Login(UserName, passwordBox.Password, KeepLogged, SelectedBackend);
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                NotifyOfPropertyChange(() => UserName);
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
