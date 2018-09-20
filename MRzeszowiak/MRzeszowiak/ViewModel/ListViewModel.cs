using GalaSoft.MvvmLight.Command;
using MRzeszowiak.Model;
using MRzeszowiak.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class ListViewModel :  INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        protected IRzeszowiak _rzeszowiakRepository;

        public bool ActivityListView => !Activity && !ErrorPanelVisible ? true : false;

        private bool activity = true;
        public bool Activity
        {
            get { return activity; }
            set
            {
                activity = value;
                OnPropertyChanged();
                OnPropertyChanged("ActivityListView");
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
                fotterActivity = value;
                OnPropertyChanged();
            }
        }

        private bool loadingNexPage;
        public bool LoadingNextPage
        {
            get { return loadingNexPage; }
            set
            {
                loadingNexPage = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("ErrorPanelVisible");
                OnPropertyChanged("Activity");
                OnPropertyChanged("ActivityListView");
            }
        }
        public bool ErrorPanelVisible => (errorMessage?.Length ?? 0) > 0 ? true : false;
        public ICommand LoadNextAdvert { get; private set; }
        protected AdvertSearchResult _lastAdvertSearchResult;
        protected AdvertSearch _lastAdvertSearch;

        public ListViewModel(IRzeszowiak RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            MessagingCenter.Subscribe<View.ListPage>(this, "LoadLastOnStartup", (sender) => {
                LoadLastOnStartup();
            });

            LoadNextAdvert = new RelayCommand(()=>LoadNextItem());
            Activity = true;
        }

        // load last add on startup
        protected void LoadLastOnStartup()
        {
            Debug.Write("LoadLastOnStartup");
            SearchExecute(new AdvertSearch(), false);
        }

        protected void LoadNextItem()
        {
            Debug.Write("LoadNextItem");
            int setting = 100;
            if (_lastAdvertSearchResult != null && _lastAdvertSearch != null && _lastAdvertSearchResult.AllPage > 1 && 
                _lastAdvertSearchResult.Page < _lastAdvertSearchResult.AllPage - 1 && _lastAdvertSearchResult.Page < setting - 1 && 
                !Activity && !FotterActivity )
            {
                _lastAdvertSearch.RequestPage = ++_lastAdvertSearchResult.Page;
                SearchExecute(_lastAdvertSearch, true);
            }
        }

        protected async void SearchExecute(AdvertSearch advertSearch, bool addLoad = false)
        {
            Debug.Write("SearchExecute");
            if (advertSearch == null)
            {
                Debug.Write("SearchExecute => advertSearch == null");
                return;
            }

            _lastAdvertSearch = advertSearch;
            
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
                foreach (var item in lastAddAdvert.AdvertShortsList)
                {
                    AdvertShortList.Add(item);
                    await Task.Delay(100);
                }     
            }
            Activity = false;
            FotterActivity = false;
        }

        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
