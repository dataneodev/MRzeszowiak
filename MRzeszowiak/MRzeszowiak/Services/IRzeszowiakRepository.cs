using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    interface IRzeszowiakRepository
    {
        Task<IList<Category>> GetCategoryList(bool ForceReload = false);
        Task<IList<AdvertShort>> GetAdvertList(AdvertSearch searchParams);
        Task<IList<AdvertShort>> GetAdvertList(); // default last add
        Task<Advert> GetAdvert(AdvertShort advertShort);
        Task<Advert> GetAdvert(int advertId);
    }
}
