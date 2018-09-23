﻿using GalaSoft.MvvmLight.Command;
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
        public bool ErrorPanelVisible => (errorMessage?.Length ?? 0) > 0 ? true : false;
        public ICommand LoadNextAdvert { get; private set; }
        public ICommand ListViewItemTapped { get; private set; }
        protected AdvertSearchResult _lastAdvertSearchResult;
        protected AdvertSearch _lastAdvertSearch;

        public ListViewModel(IRzeszowiak RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            MessagingCenter.Subscribe<View.ListPage>(this, "LoadLastOnStartup", (sender) => {
                LoadLastOnStartup();
            });

            MessagingCenter.Subscribe<View.ListPage>(this, "RefreshList", (sender) => {
                RefreshList();
            });

            MessagingCenter.Subscribe<View.PreviewPage>(this, "PreViewFowardRequest", (sender) =>
            {

            });

            MessagingCenter.Subscribe<View.PreviewPage>(this, "PreViewBackRequest", (sender) =>
            {

            });

            LoadNextAdvert = new RelayCommand(async ()=> await LoadNextItem());
            ListViewItemTapped = new RelayCommand<AdvertShort>((item) => ListViewTapped(item));
            Activity = true;
        }

        protected async void ListViewTapped(AdvertShort advertShort)
        {

        }

        protected void LoadPreviewFoward(AdvertShort advertShort)
        {
            if(advertShort == null)
            {
                Debug.Write("LoadPreviewFoward => advertShort == null");
                return;
            }

            var index = AdvertShortList?.IndexOf(advertShort) ?? -1;
            if( index != -1 && index < (AdvertShortList?.Count ?? 0) - 1)
            {

            }
        }

        // refreshing list
        protected async void RefreshList()
        {
            if(_lastAdvertSearch != null)
                await SearchExecute(_lastAdvertSearch, false);
        }

        // load last add on startup
        protected async void LoadLastOnStartup()
        {
            //Debug.Write("LoadLastOnStartup");
            await SearchExecute(new AdvertSearch(), false);
        }

        protected async Task<bool> LoadNextItem()
        {
            Debug.Write("LoadNextItem");
            int setting = 100;
            if (_lastAdvertSearchResult != null && _lastAdvertSearch != null && _lastAdvertSearchResult.AllPage > 1 && 
                _lastAdvertSearchResult.Page < _lastAdvertSearchResult.AllPage - 1 && _lastAdvertSearchResult.Page < setting - 1 && 
                !Activity && !FotterActivity )
            {
                _lastAdvertSearch.RequestPage = ++_lastAdvertSearchResult.Page;
                return await SearchExecute(_lastAdvertSearch, true).ConfigureAwait(false);
            }
            return false;
        }

        protected async Task<bool> SearchExecute(AdvertSearch advertSearch, bool addLoad = false)
        {
            Debug.Write("SearchExecute");
            if (advertSearch == null)
            {
                Debug.Write("SearchExecute => advertSearch == null");
                return false;
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

        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
