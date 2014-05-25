using Caliburn.Micro;
using MyHoard.Models;
using MyHoard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.ViewModels
{
    public class SearchViewModel: ViewModelBase
    {
        private ItemService itemService;
        private List<Item> items;
        private string searchText;
        private ObservableCollection<Item> allItems = new ObservableCollection<Item>();
        private ObservableCollection<Item> titleItems = new ObservableCollection<Item>();
        private ObservableCollection<Item> descriptionItems = new ObservableCollection<Item>();
        private Item selectedItem;

        public SearchViewModel(INavigationService navigationService, CollectionService collectionService, ItemService itemService)
            : base(navigationService, collectionService)

        {
            this.itemService = itemService;
        }

        public ObservableCollection<Item> AllItems
        {
            get { return allItems; }
            set 
            { 
                allItems = value;
                NotifyOfPropertyChange(() => AllItems);
            }
        }

        public ObservableCollection<Item> TitleItems
        {
            get { return titleItems; }
            set
            {
                titleItems = value;
                NotifyOfPropertyChange(() => TitleItems);
            }
        }

        public ObservableCollection<Item> DescriptionItems
        {
            get { return descriptionItems; }
            set
            {
                descriptionItems = value;
                NotifyOfPropertyChange(() => DescriptionItems);
            }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            items = itemService.ItemListWithThumbnails();
        }

        public void SearchChanged()
        {
            if(SearchText.Length>1)
            {
                AllItems = new ObservableCollection<Item>(items.Where(x => Contains(x.Description, SearchText, StringComparison.OrdinalIgnoreCase) || Contains(x.Name, SearchText, StringComparison.OrdinalIgnoreCase)));
                TitleItems = new ObservableCollection<Item>(items.Where(x => Contains(x.Name, SearchText, StringComparison.OrdinalIgnoreCase)));
                DescriptionItems = new ObservableCollection<Item>(items.Where(x => Contains(x.Description, SearchText, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                AllItems.Clear();
                TitleItems.Clear();
                DescriptionItems.Clear();
            }
        }

        public void ItemDetails()
        {
            NavigationService.UriFor<ItemDetailsViewModel>().WithParam(x => x.ItemId, SelectedItem.Id).Navigate();
        }

        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    
    }
}
