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

        Task SetDBPath(string dbPath);
        Task<bool> SaveSettingAsync();

        Task<DateTime> LastMailSendDateAsync(Advert advert);
        Task<bool> UpdateSendMailNoticeAsync(Advert advert);

        Task<bool> GetFavoriteAdvertListDBAsync(IList<AdvertShort> list);
        Task<Advert> GetFavoriteAdvertDBAsync(AdvertShort advertShort);
        Task<bool> InsertOrUpdateAdvertDBAsync(Advert advert);
        Task<bool> DeleteAdvertDBAsync(Advert advert);
        Task<bool> DeleteAdvertAllDBAsync();
        Task<bool> IsAdvertInDBAsync(Advert advert);

        //Task<IList<AdvertSearch>> GetFavoriteAdvertSearchListDB();
        //Task<bool> InsertOrUpdateAdvertSearchDB(AdvertSearch advert);
        //Task<bool> DeleteAdvertDB(AdvertSearch advert);
    }
}
