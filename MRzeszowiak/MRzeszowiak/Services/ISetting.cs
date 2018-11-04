using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    public interface ISetting
    {
        bool AutoSaveDB { get; set; }
        string UpdateServerUrl { get; }
        string GetAppName { get; }
        string GetAppNameAndVersion { get; }
        float GetAppVersion { get; }
        string GetRzeszowiakBaseURL { get; }
        string GetProjectBaseURL { get; }
        string UserEmail { get; set; }
        bool IsUserMailCorrect { get; }
        byte MaxScrollingAutoLoadPage { get; set; }
        AdvertSearch AutostartAdvertSearch { get; set; }

        void SetDBPath(string dbPath);

        DateTime LastMailSendDate(Advert advert);
        bool UpdateSendMailNotice(Advert advert);

        IEnumerable<AdvertShort> GetFavoriteAdvertListDB();
        bool InsertOrUpdateAdvertDB(Advert advert);
        bool DeleteAdvertDB(Advert advert);
        bool IsAdvertInDB(Advert advert);

        //Task<IList<AdvertSearch>> GetFavoriteAdvertSearchListDB();
        //Task<bool> InsertOrUpdateAdvertSearchDB(AdvertSearch advert);
        //Task<bool> DeleteAdvertDB(AdvertSearch advert);
    }
}
