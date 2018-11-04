using MRzeszowiak.Model;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class SearchViewModel : BaseViewModel, INavigationAware
    {
        protected readonly INavigationService _navigationService;

        private string searchPattern;
        public string SearchPattern
        {
            get { return searchPattern; }
            set
            {
                searchPattern = value;
                OnPropertyChanged();
            }
        }

        private Category sendCategory;
        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
                if(value == null)
                {
                    PriceMin = 0;
                    PriceMax = 0;
                    Sort = SortType.dateadd;
                }
            }
        }

        public IList<AddType> DateAddList => Array.AsReadOnly<AddType>((AddType[])Enum.GetValues(typeof(AddType)));
        public IList<SortType> SortList => Array.AsReadOnly<SortType>((SortType[])Enum.GetValues(typeof(SortType)));
        private AddType dateAdd = AddType.all;
        public AddType DateAdd
        {
            get{ return dateAdd; }
            set
            {
                dateAdd = value;
                OnPropertyChanged();
            }
        }

        private SortType sort = SortType.dateadd;
        public SortType Sort
        {
            get { return sort; }
            set
            {
                sort = value;
                OnPropertyChanged();
            }
        }

        private int? priceMin;
        public int PriceMin
        {
            get { return priceMin ?? 0; }
            set
            {
                if (value == 0)
                    priceMin = null;
                else
                    priceMin = value;                  
                OnPropertyChanged();
            }
        }

        private int? priceMax;
        public int PriceMax
        {
            get { return priceMax ?? 0; }
            set
            {
                if (value == 0)
                    priceMax = null;
                else
                    priceMax = value;
                OnPropertyChanged();
            }
        }

        private bool settingMode = false;
        public bool SettingMode
        {
            get { return settingMode; }
            set
            {
                settingMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand ButtonCancelTappped { get; private set; }
        public ICommand ButtonSearchTappped { get; private set; }
        public ICommand ButtonCategorySelectTappped { get; private set; }
        public ICommand AddTypeChange { get; private set; }
        public ICommand SortTypeChange { get; private set; }

        public SearchViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            ButtonSearchTappped = new Command(SearchExecute);
            ButtonCancelTappped = new Command(CancelExecute);
            ButtonCategorySelectTappped = new Command(() =>
            {
                var parameters = new NavigationParameters()
                {
                    {"SelectedCategory", SelectedCategory }
                };
                _navigationService.NavigateAsync("CategorySelectPopup", parameters);
            });
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedCategory"))
                SelectedCategory = parameters["SelectedCategory"] as Category;

            if (parameters.ContainsKey("SearchRecord"))
            {
                if (!(parameters["SearchRecord"] is AdvertSearch reciveSearchRecord))
                    reciveSearchRecord = new AdvertSearch();
                this.SearchPattern = reciveSearchRecord.SearchPattern;
                this.SelectedCategory = reciveSearchRecord.CategorySearch;
                this.sendCategory = reciveSearchRecord.CategorySearch;
                this.DateAdd = reciveSearchRecord.DateAdd;
                this.Sort = reciveSearchRecord.Sort;
                this.PriceMin = reciveSearchRecord.PriceMin ?? 0;
                this.PriceMax = reciveSearchRecord.PriceMax ?? 0;
            }

            if (parameters.ContainsKey("SettingMode"))
                if (parameters["SettingMode"] is bool SettingM)
                    SettingMode = SettingM;
        }    
        
        async void SearchExecute()
        {
            var advertSearch = new AdvertSearch()
            {
                SearchPattern = this.SearchPattern,
                CategorySearch = this.SelectedCategory,
                DateAdd = this.DateAdd,
                Sort = this.Sort,
                PriceMin = this.priceMin,
                PriceMax = this.priceMax,
            };

            var parameters = new NavigationParameters()
            {
                    {"SearchRecord", advertSearch }
            };
            await _navigationService.GoBackAsync(parameters);
        }

        async void CancelExecute()
        {

            var advertSearch = new AdvertSearch();
            if (!SettingMode)
                advertSearch.CategorySearch = this.sendCategory;

            var parameters = new NavigationParameters()
            {
                    {"SearchRecord", advertSearch }
            };
            await _navigationService.GoBackAsync(parameters);
        }
    }
}
