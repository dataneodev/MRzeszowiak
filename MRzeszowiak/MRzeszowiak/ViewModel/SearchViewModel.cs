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
        protected INavigationService _navigationService;

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

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public IList<AddType> DateAdd
        {
            get => Array.AsReadOnly<AddType>((AddType[])Enum.GetValues(typeof(AddType)));
        }

        public IList<SortType> SortList
        {
            get => Array.AsReadOnly<SortType>((SortType[])Enum.GetValues(typeof(SortType)));
        }

        private int? priceMin;
        public int? PriceMin
        {
            get { return priceMin; }
            set
            {
                priceMin = value;
                OnPropertyChanged();
            }
        }

        private int? priceMax;
        public int? PriceMax
        {
            get { return priceMax; }
            set
            {
                priceMax = value;
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
            ButtonCancelTappped = new Command(async () => { await _navigationService.GoBackAsync(); });
            ButtonCategorySelectTappped = new Command(() =>
            {
                var parameters = new NavigationParameters()
                {
                    {"SelectedCategory", SelectedCategory }
                };
                _navigationService.NavigateAsync("SearchPopup", parameters);
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatingTo(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedCategory"))
                if (parameters["SelectedCategory"] is Category category)
                    SelectedCategory = category;
        }            
    }
}
