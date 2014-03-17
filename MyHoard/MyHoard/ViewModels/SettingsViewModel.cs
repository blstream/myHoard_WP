using Caliburn.Micro;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.ViewModels
{
    public class SettingsViewModel:ViewModelBase
    {
        public SettingsViewModel(INavigationService navigationService, CollectionService collectionService)
            : base(navigationService, collectionService)
        {
            
        }

        public void Register()
        {

        }
    }
}
