using MRzeszowiak.Model;
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

            Tuple<int, int> getCatIDForSearch(Category cat)
            {
                if (cat == null) return new Tuple<int, int>(0, 0);


                return new Tuple<int, int>(1,1);
            }

            // preparing URL request
            string urlRequest = String.Empty;
            // 4 scenario 
            //select category without search
            if(searchParams.SearchPattern.Length == 0 && searchParams.AdvertID == 0 && searchParams.Category != null)
            {

            }

            // search string
            if (searchParams.SearchPattern.Length > 0)
            {

            }

            // search id
            if (searchParams.AdvertID != 0)
                urlRequest = $"{RZESZOWIAK_BASE_URL}szukaj/numer:{searchParams.AdvertID}";

            // last add 
            if (searchParams.SearchPattern.Length == 0 && searchParams.AdvertID == 0 && searchParams.Category == null)
                urlRequest = RZESZOWIAK_BASE_URL;

            // 
            if(urlRequest == String.Empty)
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

                    }
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync();

                    }
                }
            }

            return Task.FromResult<IList<AdvertShort>>(result);

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
