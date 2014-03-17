using Caliburn.Micro;
using MyHoard.Services;
using MyHoard.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyHoard.ViewModels
{
    public class RegisterViewModel : ViewModelBase 
    {

        private Dictionary<string, string> backends;
        private bool canRegister;
        private Visibility arePasswordRequirementsVisible;
        private string userName;
        private string email;
        private string selectedBackend;
        private PasswordBox passwordBox;
        private PasswordBox confirmPasswordBox;
        

        public RegisterViewModel(INavigationService navigationService, CollectionService collectionService)
            : base(navigationService, collectionService)
        {
            Backends = new Dictionary<string, string>()
            {
                {"Python","http://78.133.154.18:8080"},
                {"Java1","http://78.133.154.39:1080"},
                {"Java2","http://78.133.154.39:2080"}
            };
            SelectedBackend = Backends.Keys.First();
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

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifyOfPropertyChange(() => Email);
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

        

        public Visibility ArePasswordRequirementsVisible
        {
            get { return arePasswordRequirementsVisible; }
            set
            {
                arePasswordRequirementsVisible = value;
                NotifyOfPropertyChange(() => ArePasswordRequirementsVisible);
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



        protected override void OnViewLoaded(object view)
        {
            passwordBox = ((RegisterView)view).Password;
            confirmPasswordBox = ((RegisterView)view).ConfirmPassword;
            passwordBox.PasswordChanged += new RoutedEventHandler(PasswordChanged);
            confirmPasswordBox.PasswordChanged += new RoutedEventHandler(PasswordChanged);
            base.OnViewLoaded(view);
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (!String.IsNullOrEmpty(passwordBox.Password) &&
                Regex.IsMatch(passwordBox.Password, "^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{5,}$", RegexOptions.IgnoreCase))
            {
                ArePasswordRequirementsVisible = Visibility.Collapsed;
            }
            else
            {
                ArePasswordRequirementsVisible = Visibility.Visible;
            }
            DataChanged();

        }

        public void DataChanged()
        {
            CanRegister = (!String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrEmpty(Email) &&
                Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$", RegexOptions.IgnoreCase) &&
                ArePasswordRequirementsVisible == Visibility.Collapsed && passwordBox.Password == confirmPasswordBox.Password);
        }

        public async void Register()
        {
           
        }



    }
}
