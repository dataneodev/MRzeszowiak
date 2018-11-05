using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRzeszowiak.Interfaces
{
    public interface ISetting
    {
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
        void SaveSetting();

        DateTime LastMailSendDate(Advert advert);
        bool UpdateSendMailNotice(Advert advert);

        bool GetFavoriteAdvertListDB(IList<AdvertShort> list);
        Advert GetFavoriteAdvertDB(AdvertShort advertShort);
        bool InsertOrUpdateAdvertDB(Advert advert);
        bool DeleteAdvertDB(Advert advert);
        bool DeleteAdvertAllDB();
        bool IsAdvertInDB(Advert advert);

        //Task<IList<AdvertSearch>> GetFavoriteAdvertSearchListDB();
        //Task<bool> InsertOrUpdateAdvertSearchDB(AdvertSearch advert);
        //Task<bool> DeleteAdvertDB(AdvertSearch advert);
    }
}
