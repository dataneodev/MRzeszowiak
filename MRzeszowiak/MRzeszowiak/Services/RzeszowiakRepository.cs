using MRzeszowiak.Model;
using MRzeszowiak.Extends;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    class RzeszowiakRepository : IRzeszowiakRepository
    {
        protected IList<Category> _lastCategoryList = new List<Category>();
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";

        // from inteface
        public Task<IList<Category>> GetCategoryList(bool ForceReload = false)
        {
            return new Task<IList<Category>>(() => new List<Category>() { new Category() });
        }
        public async Task<IList<AdvertShort>> GetAdvertListAsync(AdvertSearch searchParams)
        {
            Debug.Write("GetAdvertListAsync(AdvertSearch searchParams)");
            var resultList = new List<AdvertShort>();
            if (searchParams == null)
            {
                Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => searchParams == null");
                return resultList;
            }

            var urlRequest = searchParams.GetURL;
            if (urlRequest == String.Empty)
            {
                Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => urlRequest == String.Empty");
                return resultList;
            }

            using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(6) })
            {
                using (HttpResponseMessage response = await client.GetAsync(urlRequest))
                {
                    if(!response.IsSuccessStatusCode)
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !response.IsSuccessStatusCode");
                        // command to user
                        return resultList;
                    }
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync();
                        result = result.CutFoward("<div id=\"query\">");
                        result = result.CutBacking("google_ad_client ");
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => " + result);
                    }
                }
            }

            return resultList;

            //var result = new List<AdvertShort>
            //{
            //    new AdvertShort()
            //    {
            //        AdverIDinRzeszowiak = 6,
            //        Title = "Auto – Naprawa Jerzy Szeliga - 37-114 Białobrzegi",
            //        DescriptionShort = "sprzedam przepiękna szklaną paterę w kolorze niebieskim[wyrób włoski].bardzo ciekawy kształt,3 cm,",
            //        DateAddString = "dziś",
            //        Price = 126,
            //        Highlighted = false,
            //        ThumbnailUrl = "http://www.rzeszowiak.pl/img/ogl/105/mini/l_10577949_0.jpg?re=1149847778",
            //    }                
            //};
        }
        public Task<IList<AdvertShort>> GetAdvertList()
        {
            return GetAdvertListAsync(new AdvertSearch());
        }
        public Task<Advert> GetAdvert(AdvertShort advertShort)
        {
            return new Task<Advert>(() => new Advert() );
        }
        public Task<Advert> GetAdvert(int advertId)
        {
            return new Task<Advert>(() =>  new Advert());
        }
    }
}
