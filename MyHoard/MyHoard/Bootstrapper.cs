using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using MyHoard.Services;
using MyHoard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace MyHoard
{
   
    public class Bootstrapper : PhoneBootstrapper
    {
        PhoneContainer container;

        protected override void Configure()
        {
            if (Execute.InDesignMode)
                return;

            container = new PhoneContainer();

            container.RegisterPhoneServices(RootFrame);
            container.PerRequest<WelcomeViewModel>();
            container.PerRequest<AddCollectionViewModel>();
            container.PerRequest<CollectionListViewModel>();
            container.PerRequest<CollectionDetailsViewModel>();
            container.PerRequest<AddItemViewModel>();
            container.PerRequest<ItemDetailsViewModel>();
            container.PerRequest<PictureViewModel>();
            container.PerRequest<PhotoChooserViewModel>();
            container.PerRequest<SettingsViewModel>();
            container.PerRequest<RegisterViewModel>();
            container.PerRequest<LoginViewModel>();
            container.PerRequest<SearchViewModel>();
            container.PerRequest<CollectionChooserViewModel>();
            container.Singleton<DatabaseService>();
            container.Singleton<CollectionService>();
            container.Singleton<ItemService>();
            container.Singleton<MediaService>();
            container.Singleton<ConfigurationService>();
            
            AddCustomConventions();
                
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<BindableAppBarButton>(
            Control.IsEnabledProperty, "DataContext", "Click");
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
        protected override void OnLaunch(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            base.OnLaunch(sender, e);
            (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color = Color.FromArgb(255,255,192,2);
            ConfigurationService configurationService = IoC.Get<ConfigurationService>();
        }
        protected override void OnClose(object sender, Microsoft.Phone.Shell.ClosingEventArgs e)
        {
            ConfigurationService configurationService = IoC.Get<ConfigurationService>();
            if (!configurationService.Configuration.KeepLogged)
            {
                configurationService.Configuration.IsLoggedIn = false;
                configurationService.SaveConfig();
            }
            IoC.Get<MediaService>().CleanIsolatedStorage();
            base.OnClose(sender, e);
        }

        protected override void OnUnhandledException(object sender, System.Windows.ApplicationUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
            MessageBox.Show(string.Format("{0}: {1}",Resources.AppResources.GeneralError,e.ToString()));
        }

           
    }
        
    
}
