using MRzeszowiak.Model;

namespace MRzeszowiak.Services
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

        bool CanSendMail(Advert advert);
        void SendMailNotice(Advert advert);

        void SetDBPath(string dbPath);

        //Task<IList<AdvertShort>> GetFavoriteAdvertListDB();
        //Task<bool> SetFavoriteAdvertListDB();

        //Task<AdvertSearch> GetAutostartAdvertSearchDB();
        //Task<bool> SetAutostartAdvertSearchDB(AdvertSearch advertSearch);

    }
}
