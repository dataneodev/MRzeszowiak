using MRzeszowiak.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Extensions;

namespace MRzeszowiak.Services
{
    public class SettingRepository : ISetting, INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _dbPath;
        private const string dbName = "mrzeszowiak.db";
        private string DbFullPath { get => Path.Combine(_dbPath ?? String.Empty, dbName); }
        [Ignore]
        public bool AutoSaveDB { get; set; } = false;
        private bool _rawObj;
        [Ignore]
        public bool RawObj
        {
            get
            {
                return _rawObj || (_dbPath?.Length ?? 0) == 0; 
            }
        }

        [Ignore]
        public string UpdateServerUrl { get => "https://script.google.com/macros/s/AKfycbxx_fFWPUjtiwBU9uFcVKhvXFLa8SjfoHZbM7DmSD_WaWmArTu1/exec"; }
        [Ignore]
        public string GetAppName { get => App.GetAppName; }
        [Ignore]
        public string GetAppNameAndVersion { get => App.GetAppNameAndVersion; }
        [Ignore]
        public float GetAppVersion { get => App.GetAppVersion; }
        [Ignore]
        public string GetRzeszowiakBaseURL { get => "http://rzeszowiak.pl";  }
        [Ignore]
        public string GetProjectBaseURL { get => "https://sites.google.com/site/dataneosoftware/polski/mrzeszowiak"; }

        private string userEmail;
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
                if ((UserEmail?.Length??0) == 0) return false;
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

        public string AutostartAdvertSearchSerialized
        {
            get => JsonConvert.SerializeObject(AutostartAdvertSearch);
            set
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    AutostartAdvertSearch = JsonConvert.DeserializeObject<AdvertSearch>(value);
                    if (AutostartAdvertSearch?.CategorySearch?.ChildCategory != null)
                        foreach (var child in AutostartAdvertSearch?.CategorySearch?.ChildCategory)
                            child.ParentCategory = AutostartAdvertSearch?.CategorySearch;
                }
                catch (System.Exception e)
                {
                    Debug.Write("AutostartAdvertSearchSerialized error => " + e.Message);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingRepository()
        {
            _rawObj = true;
        }
        public SettingRepository(bool RawObj)
        {
            _rawObj = RawObj;
        }

        public DateTime LastMailSendDate(Advert advert)
        {
            if (RawObj || advert == null) return DateTime.Now.AddYears(-1);
            Debug.Write("LastMailSendDate");
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<MailAdvertDB>();
                var tab = conn.Table<MailAdvertDB>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
                if (tab.Count() > 0)
                    return tab.First().MailSendDateTime.ToLocalTime();
            }
            return DateTime.Now.AddYears(-1);
        }

        public bool UpdateSendMailNotice(Advert advert)
        {
            if (RawObj || advert == null) return false;
            Debug.Write("UpdateSendMailNotice");
            if (advert == null) return false;
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<MailAdvertDB>();
                var tab = conn.Table<MailAdvertDB>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak) ;
                if(tab.Count() == 0)
                {
                    var dbInsert = new MailAdvertDB
                    {
                        AdverIDinRzeszowiak = advert.AdverIDinRzeszowiak,
                        MailSendDateTime = DateTime.Now,
                    };

                    if (conn.Insert(dbInsert) == 0)
                        return false;
                }    
                else
                {
                    var exists = tab.First();
                    exists.MailSendDateTime = DateTime.Now;
                    conn.Update(exists);
                }  
            }
            return true;
        }

        //
        public class MailAdvertDB
        {
            [PrimaryKey, AutoIncrement]
            public int IdDb { get; set; }
            public int AdverIDinRzeszowiak { get; set; }
            public DateTime MailSendDateTime { get; set; }
        }

        public IEnumerable<AdvertShort> GetFavoriteAdvertListDB()
        {
            if (RawObj) new List<AdvertShort>();
            Debug.Write("GetFavoriteAdvertListDB");

            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<Advert>();
                return conn.Table<Advert>().OrderByDescending(x => x.IdDb);
            }
        }

        public bool InsertOrUpdateAdvertDB(Advert advert)
        {
            if (RawObj || advert == null) return false; ;
            Debug.Write("InsertOrUpdateAdvertDB");
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<Advert>();
                var tab = conn.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
                if (tab.Count() == 0)
                {
                    conn.InsertWithChildren(advert); 
                }
                else
                {
                    advert.IdDb = tab.First().IdDb;
                    conn.InsertOrReplaceWithChildren(advert);
                }
            }
            return true;
        }

        public bool DeleteAdvertDB(Advert advert)
        {
            if (RawObj || advert == null) return false; ;
            Debug.Write("DeleteAdvertDB");
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<Advert>();
                var tab = conn.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
                if (tab.Count() >= 1)
                {
                    advert.IdDb = tab.First().IdDb;
                    conn.Delete(advert);
                }
                else return false;
            }
            return true;
        }

        public bool IsAdvertInDB(Advert advert)
        {
            if (RawObj || advert == null) return false; ;
            Debug.Write("IsAdvertInDB");
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<Advert>();
                var tab = conn.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
                if (tab.Count() >= 1)
                    return true;
            }
            return false;
        }

        public void SetDBPath(string dbPath)
        {
            _dbPath = dbPath;
            if (_dbPath?.Length > 0)
                LoadSetting();   
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (!RawObj)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                Thread saveThread = new Thread(SaveSetting);
                saveThread.Start();
            }
        }

        protected void LoadSetting()
        {
            if (RawObj) return;
            Debug.Write("LoadSettingAsync");
            using (var conn = new SQLite.SQLiteConnection(DbFullPath))
            {
                conn.CreateTable<SettingRepository>();
                if (conn.Table<SettingRepository>().Count() == 0)
                    return;

                var res = conn.Get<SettingRepository>(1);
                foreach (PropertyInfo property in typeof(SettingRepository).GetProperties())
                    if (property.CanWrite)
                        property.SetValue(this, property.GetValue(res, null), null); 
            }
        }

        private volatile int _session;
        private static readonly object SyncObject = new object();
        protected void SaveSetting()
        {
            if (!AutoSaveDB || RawObj) return;
            // prevent from to many save

            Random rnd = new Random();
            int localSession = rnd.Next(0,99999999);
            _session = localSession;
            Thread.Sleep(750);
            if (_session != localSession) return;

            Debug.Write("SaveSetting");
            lock(SyncObject)
            {
                using (var conn = new SQLite.SQLiteConnection(DbFullPath))
                {
                    _rawObj = true;
                    conn.CreateTable<SettingRepository>();
                    if (conn.Table<SettingRepository>().Count() == 0)
                        conn.Insert(this);
                    else
                        conn.Update(this);
                    _rawObj = false;
                }
            }
            Debug.WriteLine("SaveSetting: " + this.Id);
        }
    }
}
