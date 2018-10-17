using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class ListViewModel : BaseViewModel, INavigationAware
    {
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        protected readonly IRzeszowiak _rzeszowiakRepository;
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialog;

        public bool ActivityListView => !Activity && !ErrorPanelVisible ? true : false;

        private bool activity = true;
        public bool Activity
        {
            get { return activity; }
            set
            {
                bool changed = activity == value ? false : true;
                activity = value;
                if (changed)
                {
                    OnPropertyChanged();
                    OnPropertyChanged("ActivityListView");
                }
            }
        }

        private bool fotterActivity;
        public bool FotterActivity
        {
            get
            {
                return fotterActivity && !Activity && !ErrorPanelVisible;
            }
            set
            {
                bool changed = fotterActivity == value ? false : true;
                fotterActivity = value;
                if (changed)
                    OnPropertyChanged();
            }
        }

        private string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                bool changed = errorMessage == value ? false : true;
                errorMessage = value;
                if (changed)
                {
                    OnPropertyChanged();
                    OnPropertyChanged("ErrorPanelVisible");
                    OnPropertyChanged("Activity");
                    OnPropertyChanged("ActivityListView");
                }
            }
        }

        public Category CurrentCategory => _lastAdvertSearch?.Category;
        public Color FilterButtonColor
        {
            get
            {
                var normalBackground = Color.FromHex("#2196F3");
                var activeBackground = Color.Yellow;
                if (_lastAdvertSearch == null) return normalBackground;
                if (((_lastAdvertSearch.SearchPattern?.Length??0) > 0) || (_lastAdvertSearch.PriceMin > 0) || 
                    (_lastAdvertSearch.PriceMax > 0) || (_lastAdvertSearch.DateAdd != AddType.all) || 
                    (_lastAdvertSearch.Sort != SortType.dateadd))
                    return activeBackground;
                return normalBackground;
            }     
        }

        public bool ErrorPanelVisible => (errorMessage?.Length ?? 0) > 0 ? true : false;
        public ICommand LoadNextAdvert { get; private set; }
        public ICommand ListViewItemTapped { get; private set; }
        public ICommand CategorySelectButtonTaped { get; private set; }
        public ICommand SearchButtonTapped { get; private set; }
        

        protected AdvertSearchResult _lastAdvertSearchResult;
        protected AdvertSearch _lastAdvertSearch;

        public ListViewModel(INavigationService navigationService, IPageDialogService pageDialog, IRzeszowiak RzeszowiakRepository)
        {
            Debug.Write("ListViewModel Contructor");
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");

            LoadNextAdvert = new Command(LoadNextItem);
            ListViewItemTapped = new Command<AdvertShort>(ListViewTappedAsync);
            CategorySelectButtonTaped = new Command(() =>
            {
                var parameters = new NavigationParameters()
                {
                    {"SelectedCategory", _lastAdvertSearch?.Category }
                };
                _navigationService.NavigateAsync("CategorySelectPopup", parameters);
            });

            SearchButtonTapped = new Command(() =>
            {
                var parameters = new NavigationParameters()
                {
                    {"SearchRecord", _lastAdvertSearch}
                };
                _navigationService.NavigateAsync("SearchPopup", parameters);
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatingTo(INavigationParameters parameters) { }
        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedCategory"))
                await CategoryUserSelectCallbackAsync((Category)parameters["SelectedCategory"]);

            if (parameters.ContainsKey("LoadAtStartup"))
                    await LoadLastOnStartup();

            if (parameters.ContainsKey("SearchRecord"))
                if (parameters["SearchRecord"] is AdvertSearch advertSearch)
                    await SearchRecordCallbackAsync(advertSearch);
        }

        public async Task CategoryUserSelectCallbackAsync(Category selCategory)
        {
            Debug.Write("CategoryUserSelectCallbackAsync");
            if (_lastAdvertSearch == null)
                _lastAdvertSearch = new AdvertSearch();
            _lastAdvertSearch.Category = selCategory;
            await SearchExecute(_lastAdvertSearch, false);
        }

        public async Task SearchRecordCallbackAsync(AdvertSearch advertSearch)
        {
            Debug.Write("SearchRecordCallbackAsync");
            await SearchExecute(advertSearch, false);
        }

        protected async Task LoadLastOnStartup()
        {
            if (_lastAdvertSearch != null)
                await SearchExecute(_lastAdvertSearch, false);
            else
                await SearchExecute(new AdvertSearch(), false);
        }

        protected async void LoadNextItem()
        {
            int setting = 100;
            if (_lastAdvertSearchResult != null && _lastAdvertSearch != null && _lastAdvertSearchResult.AllPage > 1 &&
                _lastAdvertSearchResult.Page < _lastAdvertSearchResult.AllPage - 1 && _lastAdvertSearchResult.Page < setting - 1 &&
                !Activity && !FotterActivity)
            {
                _lastAdvertSearch.RequestPage = ++_lastAdvertSearchResult.Page;
                await SearchExecute(_lastAdvertSearch, true).ConfigureAwait(false);
                return;
            }
            return;
        }

        protected async void ListViewTappedAsync(AdvertShort advertShort)
        {
            Debug.Write("ListViewTappedAsync");
            if (advertShort == null)
            {
                Debug.Write("ListViewTappedAsync => advertShort == null");
                return;
            }

            var navigationParams = new NavigationParameters
            {
                { "AdvertShort", advertShort }
            };
            await _navigationService.NavigateAsync("PreviewPage", navigationParams, useModalNavigation: true, animated: false);
        }

        protected async Task<bool> SearchExecute(AdvertSearch advertSearch, bool addLoad = false)
        {
            Debug.Write("SearchExecute");
            Activity = true;
            ErrorMessage = String.Empty;

            _lastAdvertSearch = advertSearch;
            OnPropertyChanged("CurrentCategory");
            OnPropertyChanged("FilterButtonColor");

            if (advertSearch == null)
            {
                Debug.Write("SearchExecute => advertSearch == null");
                return false;
            }

            if (addLoad)
            {
                FotterActivity = true;
                Activity = false;
            }

            else
            {
                Activity = true;
                FotterActivity = false;
                AdvertShortList.Clear();
            }

            var lastAddAdvert = await _rzeszowiakRepository.GetAdvertListAsync(advertSearch);

            if (!lastAddAdvert.Correct)
            {
                if(!addLoad)
                    ErrorMessage = "Błąd podczas ładowania strony.\n" + lastAddAdvert.ErrorMessage;
            }
            else
            {
                _lastAdvertSearchResult = lastAddAdvert;

                var adverId = new List<int>();
                foreach (var item in AdvertShortList)
                    adverId.Add(item.AdverIDinRzeszowiak);

                foreach (var item in lastAddAdvert.AdvertShortsList)
                {
                    if (adverId.IndexOf(item.AdverIDinRzeszowiak) != -1) continue;
                    AdvertShortList.Add(item);
                    await Task.Delay(50);
                }
            }
            Activity = false;
            FotterActivity = false;
            return true;
        }
    }
}
