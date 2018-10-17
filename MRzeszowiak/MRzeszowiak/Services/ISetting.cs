using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    public interface ISetting
    {
        string UserEmail { get; set; }
        byte MaxScrollingAutoLoadPage { get; set; }
        void SetDBPath(string dbPath);

        //Task<IList<AdvertShort>> GetFavoriteAdvertListDB();
        //Task<bool> SetFavoriteAdvertListDB();

        //Task<AdvertSearch> GetAutostartAdvertSearchDB();
        //Task<bool> SetAutostartAdvertSearchDB(AdvertSearch advertSearch);

        event EventHandler<IDBEventsArgs> OnPropertyChange;
    }

    public interface IDBEventsArgs
    {
        string PropertyChangeName { get; }
    }
}
