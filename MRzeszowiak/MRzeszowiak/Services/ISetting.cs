using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    public interface ISetting
    {
        string UpdateServerUrl { get; }
        string GetAppName { get; }
        float GetAppVersion { get; }
        string GetRzeszowiakBaseURL { get; }
        string GetProjectBaseURL { get; }

        string UserEmail { get; set; }
        byte MaxScrollingAutoLoadPage { get; set; }
        void SetDBPath(string dbPath);
        AdvertSearch AutostartAdvertSearch { get; set; }

        //Task<IList<AdvertShort>> GetFavoriteAdvertListDB();
        //Task<bool> SetFavoriteAdvertListDB();

        //Task<AdvertSearch> GetAutostartAdvertSearchDB();
        //Task<bool> SetAutostartAdvertSearchDB(AdvertSearch advertSearch);

    }
}
