using MRzeszowiak.Extends;
using MRzeszowiak.Interfaces;
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

namespace MRzeszowiak.Services
{
    public class SettingRepository : ISetting
    {
        private SQLiteAsyncConnection _sqliteConnection;
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        private string _dbPath;
        private const string dbName = "mrzeszowiak.db";
        private string DbFullPath { get => _dbPath?.Length > 0 ? Path.Combine(_dbPath, dbName) : String.Empty; }
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
        public string UserEmail { get; set; } = String.Empty;
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
        public byte MaxScrollingAutoLoadPage { get; set; } = 10;
        [Ignore]
        public AdvertSearch AutostartAdvertSearch { get; set; } = new AdvertSearch();
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

        public async Task<DateTime> LastMailSendDateAsync(Advert advert)
        {
            if (advert == null) return DateTime.Now.AddYears(-1);
            Debug.Write("LastMailSendDate");
            if (_sqliteConnection == null)
            {
                Debug.Write("UpdateSendMailNoticeAsync => sqlite connection is not open");
                return DateTime.Now.AddYears(-1);
            }
            await _sqliteConnection.CreateTableAsync<MailAdvertDB>();
            var tab = _sqliteConnection.Table<MailAdvertDB>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
            if (await tab.CountAsync() > 0)
                return (await tab.FirstAsync()).MailSendDateTime;
            return DateTime.Now.AddYears(-1);
        }

        public async Task<bool> UpdateSendMailNoticeAsync(Advert advert)
        {
            if (advert == null) return false;
            Debug.Write("UpdateSendMailNoticeAsync");
            if (advert == null) return false;
            if (_sqliteConnection == null)
            {
                Debug.Write("UpdateSendMailNoticeAsync => sqlite connection is not open");
                return false;
            }
            await _sqliteConnection.CreateTableAsync<MailAdvertDB>();
            var tab = _sqliteConnection.Table<MailAdvertDB>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
            if (await tab.CountAsync() == 0)
            {
                var dbInsert = new MailAdvertDB
                {
                    AdverIDinRzeszowiak = advert.AdverIDinRzeszowiak,
                    MailSendDateTime = DateTime.Now,
                };
                if (await _sqliteConnection.InsertAsync(dbInsert) == 0)
                    return false;
            }
            else
            {
                var exists = await tab.FirstAsync();
                exists.MailSendDateTime = DateTime.Now;
                await _sqliteConnection.UpdateAsync(exists);
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

        public async Task<bool> GetFavoriteAdvertListDBAsync(IList<AdvertShort> list)
        {
            if (list == null) return false;
            Debug.Write("GetFavoriteAdvertListDBAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("GetFavoriteAdvertDBAsync => sqlite connection is not open");
                return false;
            }

            await _sqliteConnection.CreateTableAsync<Advert>();
            var result = await _sqliteConnection.Table<Advert>().OrderByDescending(x => x.IdDb).ToListAsync();
            bool rowEven = false;
            if (result != null)
                foreach (var item in result)
                {
                    item.RowEven = rowEven;
                    list.Add(item);
                    rowEven = !rowEven;
                }
            return true;
        }

        //favorite adver
        public async Task<Advert> GetFavoriteAdvertDBAsync(AdvertShort advertShort)
        {
            if (advertShort == null) return null;
            Debug.Write("GetFavoriteAdvertDBAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("GetFavoriteAdvertDBAsync => sqlite connection is not open");
                return null;
            }

            await _sqliteConnection.CreateTableAsync<Advert>();
            var tab = _sqliteConnection.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advertShort.AdverIDinRzeszowiak);
            if (await tab.CountAsync() == 0)
                return null;
            else
                return await tab.FirstAsync();
        }

            //favorite adver
        public async Task<bool> InsertOrUpdateAdvertDBAsync(Advert advert)
        {
            if (advert == null) return false; ;
            Debug.Write("InsertOrUpdateAdvertDBAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("InsertOrUpdateAdvertDBAsync => sqlite connection is not open");
                return false;
            }
            await _sqliteConnection.CreateTableAsync<Advert>();
            var tab = _sqliteConnection.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
            if (await tab.CountAsync() == 0)
            {
                await _sqliteConnection.InsertAsync(advert);
            }
            else
            {
                advert.IdDb = (await tab.FirstAsync()).IdDb;
                await _sqliteConnection.InsertOrReplaceAsync(advert);
            }
            return true;
        }
        //favorite adver
        public async Task<bool> DeleteAdvertDBAsync(Advert advert)
        {
            if (advert == null) return false; ;
            Debug.Write("DeleteAdvertDBAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("DeleteAdvertDBAsync => sqlite connection is not open");
                return false;
            }

            await _sqliteConnection.CreateTableAsync<Advert>();
            var tab = _sqliteConnection.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
            if (await tab.CountAsync() >= 1)
            {
                advert.IdDb = (await tab.FirstAsync()).IdDb;
                await _sqliteConnection.DeleteAsync(advert);
            }
            else return false;
            return true;
        }
        //favorite adver
        public async Task<bool> DeleteAdvertAllDBAsync()
        {
             Debug.Write("DeleteAdvertAllDBAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("DeleteAdvertAllDBAsync => sqlite connection is not open");
                return false;
            }
            await _sqliteConnection.DeleteAllAsync<Advert>();
            return true;
        }
        //favorite adver
        public async Task<bool> IsAdvertInDBAsync(Advert advert)
        {
            if (advert == null) return false; ;
            Debug.Write("IsAdvertInDBAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("IsAdvertInDBAsync => sqlite connection is not open");
                return false;
            }
            await _sqliteConnection.CreateTableAsync<Advert>();
            var tab = _sqliteConnection.Table<Advert>().Where(v => v.AdverIDinRzeszowiak == advert.AdverIDinRzeszowiak);
            if (await tab.CountAsync() >= 1)
                return true;
            return false;
        }

        public async Task SetDBPath(string dbPath)
        {
            _dbPath = dbPath;
            if (!PathExtensions.IsValidPath(DbFullPath, true))
            {
                Debug.Write("SetDBPath => !PathExtensions.IsValidPath(DbFullPath)");
                _dbPath = String.Empty;
                return;
            }
            if(_sqliteConnection == null)
            {
                _sqliteConnection = new SQLiteAsyncConnection(DbFullPath);
                await LoadSettingAsync();
            }                  
        }

        protected async Task<bool> LoadSettingAsync()
        {
            Debug.Write("LoadSettingAsync");
            if (_sqliteConnection == null)
            {
                Debug.Write("LoadSettingAsync => sqlite connection is not open");
                return false;
            }

            await _sqliteConnection.CreateTableAsync<SettingRepository>();
            if (await _sqliteConnection.Table<SettingRepository>().CountAsync() == 0)
                return false;
            var res = await _sqliteConnection.GetAsync<SettingRepository>(1);
            foreach (PropertyInfo property in typeof(SettingRepository).GetProperties())
                if (property.CanWrite)
                    property.SetValue(this, property.GetValue(res, null), null);
            return true;
        }

        public async Task<bool> SaveSettingAsync()
        {
            Debug.Write("SaveSettingAsync");
            if(_sqliteConnection == null)
            {
                Debug.Write("SaveSettingAsync => sqlite connection is not open");
                return false;
            }

            await _sqliteConnection.CreateTableAsync<SettingRepository>();
            if (await _sqliteConnection.Table<SettingRepository>().CountAsync() == 0)
                await _sqliteConnection.InsertAsync(this);
            else
                await _sqliteConnection.UpdateAsync(this);
            Debug.WriteLine("SaveSettingAsync: " + this.Id);
            return true;
        }

        public async void Dispose()
        {
            await _sqliteConnection.CloseAsync();
        }
    }
}
