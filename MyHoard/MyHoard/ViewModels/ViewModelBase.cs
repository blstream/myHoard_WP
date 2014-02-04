using Caliburn.Micro;
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

        public ViewModelBase(INavigationService navigationService)
        {
            this.NavigationService = navigationService;
        }

    }
}
