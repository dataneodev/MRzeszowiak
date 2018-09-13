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

            IEnumerable<AdvertShort> ProcessResponse(string ResponseHTMLBody)
            {
                if (ResponseHTMLBody.Length == 0) yield break;

                // promo now
                int promoPos, normalPos;
                bool rowEven = false;
                do
                {
                    promoPos = ResponseHTMLBody.IndexOf("promobox-title-left");
                    normalPos = ResponseHTMLBody.IndexOf("normalbox-title-left");

                    int nextPos = Math.Min(promoPos, normalPos);
                    nextPos = (nextPos == -1 && promoPos > -1) ? promoPos : nextPos;
                    nextPos = (nextPos == -1 && normalPos > -1) ? normalPos : nextPos;
                    if (nextPos == -1) yield break;
                    bool highlighted = (nextPos == promoPos) ? true : false;
                    ResponseHTMLBody = ResponseHTMLBody.Substring(nextPos);

                    //string aid = ResponseHTMLBody.GetItem("id=\"o", "\">" );
                    //url
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("<a href=\"/");
                    string aUrl = ResponseHTMLBody.CutBacking("\">").Trim();
                    //title
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("\">");
                    string aTitle = ResponseHTMLBody.CutBacking("</a>").CutFoward(".").Trim();
                    //price
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("cena: <strong>");
                    string aPrice = ResponseHTMLBody.CutBacking("zł</strong>").Trim();
                    if(!Int32.TryParse(aPrice, out int aPriceInt))
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !Int32.TryParse(aPrice, out int aPriceInt)");
                        aPriceInt = 0;
                    }
                    //id
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("id=\"zr");
                    string aId = ResponseHTMLBody.CutBacking("\">").Trim();
                    if (!Int32.TryParse(aId, out int aIdInt))
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !Int32.TryParse(aId, out int aIdInt))");
                        yield break;
                    }
                    //thumb
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("src=\"");
                    string aThumb = ResponseHTMLBody.CutBacking("\"").Trim();
                    //desc
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("window.location");
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("\">");
                    string aDesc = ResponseHTMLBody.CutBacking("</div>").Trim().StripHTML();
                    if (aDesc.Length > 100)
                        aDesc = aDesc.Substring(0, 100) + "...";
                    //add
                    ResponseHTMLBody = ResponseHTMLBody.CutFoward("Dodane : <b>");
                    string aAdd = ResponseHTMLBody.CutBacking("</b>").Trim().Replace("  ", " ");

                    yield return new AdvertShort()
                    {
                        AdverIDinRzeszowiak = aIdInt,
                        Title = aTitle,
                        URLPath = aUrl,
                        ThumbnailUrl = aThumb,
                        DescriptionShort = aDesc,
                        DateAddString = aAdd,
                        Price = aPriceInt,
                        Category = searchParams.Category,
                        Highlighted = highlighted,
                        RowEven = rowEven
                    };
                    rowEven = !rowEven;
                } while (promoPos != -1 || normalPos != -1);
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
                        var byteArray = await content.ReadAsByteArrayAsync();

                        Encoding iso = Encoding.GetEncoding("ISO-8859-2");
                        Encoding utf8 = Encoding.UTF8;
                        byte[] utf8Bytes = Encoding.Convert(iso, utf8, byteArray);
                        string responseString = System.Net.WebUtility.HtmlDecode( utf8.GetString(utf8Bytes) );

                        foreach (var item in ProcessResponse(responseString)) resultList.Add(item);
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
