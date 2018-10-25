using MRzeszowiak.Model;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MRzeszowiak.Services
{
    public class SettingRepository : ISetting, INotifyPropertyChanged
    {
        private string _dbPath;
        private const string dbName = "mrzeszowiak.db";
        private string DbFullPath { get => Path.Combine(_dbPath, dbName); }
        private bool loadingDB = false;
        [Ignore]
        public string UpdateServerUrl { get => "https://script.google.com/macros/s/AKfycbxx_fFWPUjtiwBU9uFcVKhvXFLa8SjfoHZbM7DmSD_WaWmArTu1/exec"; }
        [Ignore]
        public string GetAppName { get => "MRzeszowiak"; }
        [Ignore]
        public string GetAppNameAndVersion { get => GetAppName + " " + GetAppVersion.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture); }
        [Ignore]
        public float GetAppVersion { get => 1.0f; }
        [Ignore]
        public string GetRzeszowiakBaseURL { get => "http://rzeszowiak.pl";  }
        [Ignore]
        public string GetProjectBaseURL { get => "https://sites.google.com/site/dataneosoftware/polski/mrzeszowiak"; }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set;}
        private string userEmail = "gkmail@gmx.cn";
        public string UserEmail
        {
            get { return userEmail; }
            set
            {
                userEmail = value;
                OnPropertyChanged();
            }
        }

        [Ignore]
        public bool IsUserMailCorrect
        {
            get
            {
                const string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
                var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return regex.IsMatch(UserEmail);
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
        [Ignore]
        public AdvertSearch AutostartAdvertSearch
        {
            get { return autostartAdvertSearch; }
            set
            {
                autostartAdvertSearch = value;
                OnPropertyChanged();
            }
        }

        [ForeignKey(typeof(AdvertSearch))]
        public int AdvertSearchId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanSendMail(Advert advert)
        {
            return true;
        }

        public void SendMailNotice(Advert advert)
        {

        }

        public void SetDBPath(string dbPath)
        {
            _dbPath = dbPath;
            if (_dbPath?.Length > 0)
            {
               // LoadSettingAsync();
                OnPropertyChanged("AutostartAdvertSearch");
            }
                
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            SaveSetting();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void LoadSettingAsync()
        {
            Debug.Write("LoadSettingAsync");
    
            loadingDB = true;
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<SettingRepository>();
                conn.CreateTable<AdvertSearch>();

                if (conn.Table<SettingRepository>().Count() == 0)
                {
                    loadingDB = false;
                    return;
                }

                var res = conn.GetWithChildren<SettingRepository>(1);
                foreach (PropertyInfo property in typeof(SettingRepository).GetProperties())
                    if (property.CanWrite)
                        property.SetValue(this, property.GetValue(res, null), null);
            }
            loadingDB = false;
        }

        public void SaveSetting()
        {
            if (loadingDB) return;
            Debug.Write("SaveSetting");
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<SettingRepository>();
                conn.CreateTable<AdvertSearch>();

                //var save = new SettingRepository();
                //foreach (PropertyInfo property in typeof(ISetting).GetProperties())
                //    if (property.CanWrite)
                //        property.SetValue(save, property.GetValue(this, null), null);

                if (conn.Table<SettingRepository>().Count() == 0)
                    conn.InsertWithChildren(this);
                else
                    conn.Update(this);

                Debug.WriteLine("SaveSetting: " + this.Id);
            }
        }
    }
}
