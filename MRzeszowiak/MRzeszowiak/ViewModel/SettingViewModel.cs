using MRzeszowiak.Model;
using MRzeszowiak.Extends;
using Prism.Navigation;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using MRzeszowiak.Interfaces;

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
                    {"SearchRecord", Setting.AutostartAdvertSearch.CloneObjectSerializable<AdvertSearch>()},
                    {"SettingMode", true}
                };
                _navigationService.NavigateAsync("SearchPopup", parameters);
            });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SearchRecord"))
                if (parameters["SearchRecord"] is AdvertSearch advertSearch)
                    Setting.AutostartAdvertSearch = advertSearch;
        }
    }
}
