using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    interface IRzeszowiakRepository
    {
        Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false, Action<string> userNotify = null);
        Task<IList<AdvertShort>> GetAdvertListAsync(AdvertSearch searchParams, Action<string> userNotify = null);
        Task<IList<AdvertShort>> GetAdvertListAsync(Action<string> userNotify = null); // default last add
        Task<Advert> GetAdvertAsync(AdvertShort advertShort, Action<string> userNotify = null);
        //Task<Advert> GetAdvertAsync(int advertId, Action<string> userNotify = null);
    }
}
