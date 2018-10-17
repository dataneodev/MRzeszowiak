using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class MenuMasterDetailViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        public List<MasterPageItem> MenuList { get; private set; } = new List<MasterPageItem>()
        {
            new MasterPageItem{Title = "Ulubione wyszukiwania", IconSource = "menu_favsearch.png", TargetPage = "FavSearchPage"  },
            new MasterPageItem{Title = "Ulubione ogłoszenia", IconSource = "menu_favadvert.png", TargetPage = "FavAdvertPage"  },
            new MasterPageItem{Title = "Ustawienia", IconSource = "menu_setting.png", TargetPage = "SettingPage"  },
            new MasterPageItem{Title = "O aplikacji", IconSource = "menu_about.png", TargetPage = "AboutPage"  },
            new MasterPageItem{Title = "www.rzeszowiak.pl", IconSource = "menu_rzeszowiak.png", TargetPage = "www.rzeszowiak.pl"  },
        };

        public ICommand MenuItemTapped { get; private set; }

        public MenuMasterDetailViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            MenuItemTapped = new Command<MasterPageItem>(MenuItemTappedm);
        }

        protected async void MenuItemTappedm(MasterPageItem item)
        {
            if (item == null) return;
            if( item.TargetPage == "www.rzeszowiak.pl")
            {
                Device.OpenUri(new Uri(App.RzeszowiakURL));
                return;
            }
            await _navigationService.NavigateAsync("MainNavigation/" + item.TargetPage, null);
        }
    }

    public class MasterPageItem
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public string TargetPage { get; set; }
    }
}
