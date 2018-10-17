using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Services
{
    class SettingRepository : ISetting
    {
        private readonly string _dbPath;

        private string userEmail = String.Empty;
        public string UserEmail
        {
            get { return userEmail; }
            set
            {
                userEmail = value;
                PropertyChange();
            }
        }

        private byte maxScrollingAutoLoadPage = 10;
        public byte MaxScrollingAutoLoadPage
        {
            get { return maxScrollingAutoLoadPage; }
            set
            {
                maxScrollingAutoLoadPage = value;
                PropertyChange();
            }
        }

        public event EventHandler<IDBEventsArgs> OnPropertyChange;

        public SettingRepository(string dbpath)
        {
            _dbPath = dbpath;
        }

        private void PropertyChange([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            OnPropertyChange?.Invoke(this, new DBEventsArgs(name));
        }
    }

    class DBEventsArgs : EventArgs, IDBEventsArgs
    {

        public string PropertyChangeName { get; }
        public DBEventsArgs(string name)
        {
            PropertyChangeName = name;
        }
    }
}
