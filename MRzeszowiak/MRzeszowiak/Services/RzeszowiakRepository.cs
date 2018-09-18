
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MRzeszowiak.Extends;
using MRzeszowiak.Model;
namespace MRzeszowiak.Services
{
    class RzeszowiakRepository : IRzeszowiakRepository
    {
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";
        protected IList<Category> _lastCategoryList = new List<Category>();
        // from inteface
        public Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false, Action<string> userNotify = null)
        {
            return new Task<IList<Category>>(() => new List<Category>() { new Category() });
        }
        public async Task<IList<AdvertShort>> GetAdvertListAsync(AdvertSearch searchParams, Action<string> userNotify = null)
        {
            Debug.Write("GetAdvertListAsync(AdvertSearch searchParams)");
            if (searchParams == null) 
                throw new NullReferenceException("AdvertSearch is null");

            var resultList = new List<AdvertShort>();
            var urlRequest = searchParams.GetURL;
            if (urlRequest.Length == 0)
            {
                Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => urlRequest.Length == 0");
                return resultList;
            }

            IEnumerable<AdvertShort> ProcessResponse(StringBuilder ResponseHTMLBody)
            {
                if (ResponseHTMLBody.Length == 0) yield break;

                int promoPos, normalPos;
                bool rowEven = false;
                do
                {
                    promoPos = ResponseHTMLBody.IndexOf("promobox-title-left", 0, true);
                    normalPos = ResponseHTMLBody.IndexOf("normalbox-title-left", 0, true);

                    int nextPos = Math.Min(promoPos, normalPos);
                    nextPos = (nextPos == -1 && promoPos > -1) ? promoPos : nextPos;
                    nextPos = (nextPos == -1 && normalPos > -1) ? normalPos : nextPos;
                    if (nextPos == -1) yield break;
                    bool highlighted = (nextPos == promoPos) ? true : false;
                    ResponseHTMLBody.Remove(0, nextPos);
                    
                    //url
                    ResponseHTMLBody.CutFoward("<a href=\"/");
                    string aUrl = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("\">", 0, true)).Trim();
                    
                    //title
                    ResponseHTMLBody.CutFoward("\">");
                    string aTitle = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("</a>", 0, true)).CutFoward(".").Trim();
                    
                    //price
                    ResponseHTMLBody.CutFoward("cena: <strong>");
                    string aPrice = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("zł</strong>", 0, true)).Trim();
                    if (!Int32.TryParse(aPrice, out int aPriceInt))
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !Int32.TryParse(aPrice, out int aPriceInt)");
                        aPriceInt = 0;
                    }
                    //id
                    ResponseHTMLBody.CutFoward("id=\"zr");
                    string aId = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("\">", 0, true)).Trim();

                    if (!Int32.TryParse(aId, out int aIdInt))
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !Int32.TryParse(aId, out int aIdInt))");
                        yield break;
                    }
                    //thumb
                    ResponseHTMLBody.CutFoward("src=\"");
                    string aThumb = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("\"", 0, true)).Trim();

                    //desc
                    ResponseHTMLBody.CutFoward("window.location");
                    ResponseHTMLBody.CutFoward("\">");
                    string aDesc = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("</div>", 0, true)).StripHTML().Trim();

                    //add
                    ResponseHTMLBody.CutFoward("Dodane : <b>");
                    string aAdd = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("</b>", 0, true)).Trim();

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
            var responseString =  await GetWebPage(searchParams.GetURL, userNotify);
            foreach (var item in ProcessResponse(responseString))
                resultList.Add(item);
            responseString.Clear();
            return resultList;
        }

        public Task<IList<AdvertShort>> GetAdvertListAsync(Action<string> userNotify = null)
        {
            return GetAdvertListAsync(new AdvertSearch(), userNotify);
        }

        public async Task<Advert> GetAdvertAsync(AdvertShort advertShort, Action<string> userNotify = null)
        {
            Debug.Write("GetAdvert(AdvertShort advertShort)");
            if (advertShort == null)
                throw new NullReferenceException("advertShort == null");

            var BodyResult = await GetWebPage(advertShort.URL);
            if(BodyResult.Length == 0)
            {
                Debug.Write("GetAdvertAsync => BodyResult.Length == 0");
                return null;
            }

            BodyResult.CutFoward("ogloszeniebox-top");
            
            string GetValue(StringBuilder bodyResult, string category)
            {
                BodyResult.CutFoward(category + "</div>");
                BodyResult.CutFoward("<div class=\"value\">");
                return BodyResult.ToString(0, BodyResult.IndexOf("</div>", 0, true)).Trim();
            }
            
            string aCategory = GetValue(BodyResult, "kategoria :");
            string aTitle = GetValue(BodyResult, "tytuł :").Trim();
            string aDateAdd = GetValue(BodyResult, "data dodania :");
            string aViews = GetValue(BodyResult, "wyświetleń :").Replace(" razy","");
            if (!Int32.TryParse(aViews, out int aViewsInt))
            {
                Debug.Write("GetAdvert(AdvertShort advertShort, Action<string> userNotify = null) => !Int32.TryParse(aViews, out int aViewsInt)");
                return null;
            }
            string aPrice = GetValue(BodyResult, "cena :").Replace(" PLN", "");
            if (!Int32.TryParse(aPrice, out int aPriceInt))
            {
                Debug.Write("GetAdvert(AdvertShort advertShort, Action<string> userNotify = null) => !Int32.TryParse(aPrice, out int aPriceInt)");
                return null;
            }
            BodyResult.CutFoward("Treść ogłoszenia");
            BodyResult.CutFoward("<div class=\"content\">");
            string aDesc = BodyResult.ToString(0, BodyResult.IndexOf("</div>", 0, true)).Replace("<br />","\n\n").StripHTML();
            aDesc = aDesc.Replace("\n\n\n", "\n").Trim();

            var additionalData = new Dictionary<string, string>();
            if (BodyResult.IndexOf("<div>Dane dodatkowe</div>", 0, true) != -1)
            {
                BodyResult.CutFoward("<div>Dane dodatkowe</div>");
                StringBuilder addData = new StringBuilder(BodyResult.ToString(0, BodyResult.IndexOf("ogloszeniebox-bottom", 0, true)));
                do
                {
                    int posLab = addData.IndexOf("<div class=\"label\">", 0, true);
                    if (posLab == -1) break;
                    addData.CutFoward("<div class=\"label\">");
                    string key = addData.ToString(0, addData.IndexOf("</div>", 0, true));
                    addData.CutFoward("</div>");
                    addData.CutFoward("\">");
                    string value = addData.ToString(0, addData.IndexOf("</div>", 0, true));
                    additionalData.Add(key, value);
                } while (true);
            }

            var pictureList = new List<string>();
            if (BodyResult.IndexOf("<div>Zdjęcia</div>", 0, true) != -1)
            {
                BodyResult.CutFoward("<div>Zdjęcia</div>");
                StringBuilder addData = new StringBuilder(BodyResult.ToString(0, BodyResult.IndexOf("ogloszeniebox-bottom", 0, true)));
                do
                {
                    int posLab = addData.IndexOf("<img src=\"/", 0, true);
                    if (posLab == -1) break;
                    addData.CutFoward("<img src=\"/");
                    var picUrl = RZESZOWIAK_BASE_URL + addData.ToString(0, addData.IndexOf("\"", 0, true)).Replace("mini/o_", "");
                    pictureList.Add(picUrl);
                } while (true);
            }

            return new Advert()
            {
                AdverIDinRzeszowiak = advertShort.AdverIDinRzeszowiak,
                Category = null,
                Title = aTitle,
                DateAddString = aDateAdd,
                ExpiredString = String.Empty,
                Views = aViewsInt,
                Price = aPriceInt,
                Highlighted = advertShort.Highlighted,
                DescriptionHTML = aDesc,
                AdditionalData = additionalData,
                ImageURLsList = pictureList,
                URLPath = advertShort.URLPath,
            };
        }

        //public Task<Advert> GetAdvertAsync(int advertId, Action<string> userNotify = null)
        //{
        //    return GetAdvertAsync(new AdvertShort() { AdverIDinRzeszowiak = advertId }, userNotify);
        //}

        protected async Task<StringBuilder> GetWebPage(string Url, Action<string> userNotify = null)
        {
            StringBuilder BodyString = new StringBuilder();
            using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(6) })
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(Url))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !response.IsSuccessStatusCode");
                            userNotify?.Invoke("Niepoprawna odpowiedź z serwera");
                            return BodyString;
                        }
                        using (HttpContent content = response.Content)
                        {
                            var byteArray = await content.ReadAsByteArrayAsync();
                            Encoding iso = Encoding.GetEncoding("ISO-8859-2");
                            Encoding utf8 = Encoding.UTF8;
                            byte[] utf8Bytes = Encoding.Convert(iso, utf8, byteArray);
                            BodyString.Append(System.Net.WebUtility.HtmlDecode(utf8.GetString(utf8Bytes))); 
                        }
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => System.Threading.Tasks.TaskCanceledException");
                    userNotify?.Invoke("Błąd połączenia. Przekroczono limit połączenia");
                    return BodyString;
                }
                catch(Exception e)
                {
                    Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => " + e.Message);
                    userNotify?.Invoke("Błąd połączenia z serwerem.");
                    return BodyString;
                }
            }
            return BodyString;
        }
    }
}
