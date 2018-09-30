
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MRzeszowiak.Extends;
using MRzeszowiak.Model;
using Xamarin.Forms;

namespace MRzeszowiak.Services
{
    class RzeszowiakRepository : IRzeszowiak
    {
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";
        protected IList<Category> _lastCategoryList = new List<Category>(); // cache
        // from inteface
        public async Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false, Action<string> userNotify = null)
        {
            if (_lastCategoryList.Count == 0 || ForceReload == true)
            {
                var HttpResult = await GetWeb.GetWebPage("http://www.rzeszowiak.pl/kontakt/");
                if (!HttpResult.Success)
                {
                    Debug.Write("GetCategoryListAsync => !HttpResult.Success");
                    return _lastCategoryList;
                }

                if (HttpResult.BodyString.Length == 0)
                {
                    Debug.Write("GetCategoryListAsync => responseString.Length == 0");
                    return _lastCategoryList;
                }

                await UpdateCategoryList(HttpResult.BodyString);
                HttpResult.BodyString.Clear();
            }

            return _lastCategoryList;
        }

        protected async Task<bool> UpdateCategoryList(StringBuilder htmlBody)
        {
            if ((htmlBody?.Length ?? 0) == 0)
            {
                Debug.Write("UpdateCategoryList => (htmlBody?.Length??0) == 0");
                return false;
            }

            if (htmlBody.IndexOf("<div class=\"menu-left-category\">", 0, true) == -1)
            {
                Debug.Write("UpdateCategoryList => no menu found");
                return false;
            }



            return true;
        }

        public async Task<AdvertSearchResult> GetAdvertListAsync(AdvertSearch searchParams, Action<string> userNotify = null)
        {
            Debug.Write("GetAdvertListAsync(AdvertSearch searchParams)");
            if (searchParams == null)
                throw new NullReferenceException("AdvertSearch is null");

            var resultList = new AdvertSearchResult()
            {
                Correct = false,
                ErrorMessage = String.Empty,
                Page = 1,
                AllPage = 1,
                AdvertShortsList = new List<AdvertShort>(),
            };

            var urlRequest = searchParams.GetURL;
            if (urlRequest.Length == 0)
            {
                Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => urlRequest.Length == 0");
                resultList.ErrorMessage = "Błędny URL";
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
                    if (ResponseHTMLBody.IndexOf("<a href=\"/", 0, true) == -1)
                    {
                        Debug.Write("Task<AdvertSearchResult> GetAdvertListAsync => url not found");
                        yield break;
                    }
                    ResponseHTMLBody.CutFoward("<a href=\"/");
                    string aUrl = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("\">", 0, true)).Trim();
                    if (aUrl.Length == 0)
                    {
                        Debug.Write("Task<AdvertSearchResult> aUrl.Length == 0");
                        yield break;
                    }

                    //title
                    ResponseHTMLBody.CutFoward("\">");
                    string aTitle = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("</a>", 0, true)).CutFoward(".").Trim();
                    if (aTitle.Length == 0)
                    {
                        Debug.Write("Task<AdvertSearchResult> aUrl.Length == 0");
                        yield break;
                    }

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

            var HttpResult = await GetWeb.GetWebPage(searchParams.GetURL);
            if (!HttpResult.Success)
            {
                Debug.Write("GetAdvertAsync => !HttpResult.Success");
                return resultList;
            }
            var responseString = HttpResult.BodyString;
            if (responseString.Length == 0)
            {
                resultList.ErrorMessage = "Sprawdź połączenie internetowe i spróbuj ponownie.";
                responseString.Clear();
                return resultList;
            }

            await UpdateCategoryList(responseString); // update category

            foreach (var item in ProcessResponse(responseString))
                resultList.AdvertShortsList.Add(item);

            if (responseString.IndexOf("<div id=\"oDnns\">Strona  ", 0, true) != -1)
            {
                responseString.CutFoward("<div id=\"oDnns\">Strona  ");
                string aPage = responseString.ToString(0, responseString.IndexOf("z", 0, true)).Trim();
                if (!Int32.TryParse(aPage, out int aPageInt))
                {
                    Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !Int32.TryParse(aPage, out int aPageInt)");
                    aPageInt = 0;
                }
                resultList.Page = aPageInt;
                responseString.CutFoward("z");

                string aPageAll = responseString.ToString(0, responseString.IndexOf("</div>", 0, true)).Trim();
                if (!Int32.TryParse(aPageAll, out int aPageAllInt))
                {
                    Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !Int32.TryParse(aPageAll, out int aPageAllInt)");
                    aPageAllInt = 0;
                }
                resultList.AllPage = aPageAllInt;
            }

            responseString.Clear();
            resultList.Correct = true;
            return resultList;
        }

        public Task<AdvertSearchResult> GetAdvertListAsync(Action<string> userNotify = null)
        {
            return GetAdvertListAsync(new AdvertSearch(), userNotify);
        }

        public async Task<Advert> GetAdvertAsync(AdvertShort advertShort, Action<string> userNotify = null)
        {
            Debug.Write("GetAdvert(AdvertShort advertShort)");
            if (advertShort == null)
                throw new NullReferenceException("advertShort == null");

            var HttpResult = await GetWeb.GetWebPage(advertShort.URL);
            if (!HttpResult.Success)
            {
                Debug.Write("GetAdvertAsync => !HttpResult.Success");
                return null;
            }
            var BodyResult = HttpResult.BodyString;
            if (BodyResult.Length == 0)
            {
                Debug.Write("GetAdvertAsync => BodyResult.Length == 0");
                return null;
            }

            await UpdateCategoryList(BodyResult); // update category

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
            string aViews = GetValue(BodyResult, "wyświetleń :").Replace(" razy", "");
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
            string aDesc = BodyResult.ToString(0, BodyResult.IndexOf("</div>", 0, true)).Replace("<br />", "\n\n").StripHTML();
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

            // image sid
            string aSsid = String.Empty;
            if (BodyResult.IndexOf("rel=\"" + advertShort.AdverIDinRzeszowiak + "|", 0, true) != -1)
            {
                BodyResult.CutFoward("rel=\"" + advertShort.AdverIDinRzeszowiak + "|");
                aSsid = BodyResult.ToString(0, BodyResult.IndexOf("\"", 0, true)).Trim();
            }
            Cookie aCookie = null;
            foreach (var item in HttpResult.CookieList)
                if (item.Name == "PHPSESSID") aCookie = item;

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
                PhoneSsid = aSsid,
                PhonePHPSSESION = aCookie,
            };
        }
    }
}
