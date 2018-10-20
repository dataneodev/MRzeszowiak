using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class SettingViewModel : BaseViewModel, INavigationAware
    {
        protected readonly INavigationService _navigationService;
        
        public ISetting Setting { get; private set; }
        public ICommand SearchButtonTapped { get; set; }


        public SettingViewModel(INavigationService navigationService, ISetting setting)
        {
            Debug.Write("SettingViewModel Contructor");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            Setting = setting ?? throw new NullReferenceException("ISetting setting == null !");

            SearchButtonTapped = new Command(() =>
            {
                Debug.Write("SearchButtonTapped");
                var parameters = new NavigationParameters()
                {
                    {"SearchRecord", Setting.AutostartAdvertSearch}
                };
                _navigationService.NavigateAsync("SearchPopup", parameters);
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatingTo(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SearchRecord"))
                if (parameters["SearchRecord"] is AdvertSearch advertSearch)
                    Setting.AutostartAdvertSearch = advertSearch;
        }

    }
}
