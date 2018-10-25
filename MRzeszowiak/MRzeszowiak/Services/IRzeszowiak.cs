using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    public interface IRzeszowiak
    {
        Task<IList<MasterCategory>> GetMasterCategoryListAsync(bool ForceReload = false);
        Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false);
        Task<AdvertSearchResult> GetAdvertListAsync(AdvertSearch searchParams, Action<string> userNotify = null);
        Task<AdvertSearchResult> GetAdvertListAsync(Action<string> userNotify = null); // default last add
        Task<Advert> GetAdvertAsync(AdvertShort advertShort, Action<string> userNotify = null);
        Task<bool> SendUserMessage(Advert advertShort, string message, string email);
    }
}
