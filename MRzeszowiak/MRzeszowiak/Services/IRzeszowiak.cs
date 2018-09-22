﻿using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    interface IRzeszowiak
    {
        Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false, Action<string> userNotify = null);
        Task<AdvertSearchResult> GetAdvertListAsync(AdvertSearch searchParams, Action<string> userNotify = null);
        Task<AdvertSearchResult> GetAdvertListAsync(Action<string> userNotify = null); // default last add
        Task<Advert> GetAdvertAsync(AdvertShort advertShort, Action<string> userNotify = null);
    }
}