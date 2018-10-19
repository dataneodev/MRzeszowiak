using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MRzeszowiak.Services
{
    class SettingRepository : ISetting, INotifyPropertyChanged
    {
        private string _dbPath;
        public string UpdateServerUrl { get => "https://script.google.com/macros/s/AKfycbxx_fFWPUjtiwBU9uFcVKhvXFLa8SjfoHZbM7DmSD_WaWmArTu1/exec"; }
        public string GetAppName { get => "MRzeszowiak"; }
        public float GetAppVersion { get => 1.0f; }
        public string GetRzeszowiakBaseURL { get => "http://rzeszowiak.pl";  }
        public string GetProjectBaseURL { get => "https://sites.google.com/site/dataneosoftware/polski/mrzeszowiak"; }

        private string userEmail = String.Empty;
        public string UserEmail
        {
            get { return userEmail; }
            set
            {
                userEmail = value;
                OnPropertyChanged();
            }
        }

        private byte maxScrollingAutoLoadPage = 10;
        public byte MaxScrollingAutoLoadPage
        {
            get { return maxScrollingAutoLoadPage; }
            set
            {
                maxScrollingAutoLoadPage = value;
                OnPropertyChanged();
            }
        }

        private AdvertSearch autostartAdvertSearch = new AdvertSearch();
        public AdvertSearch AutostartAdvertSearch
        {
            get { return autostartAdvertSearch; }
            set
            {
                autostartAdvertSearch = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void SetDBPath(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
