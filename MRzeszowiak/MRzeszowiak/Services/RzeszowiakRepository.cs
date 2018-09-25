
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        protected IList<Category> _lastCategoryList = new List<Category>();
        // from inteface
        public Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false, Action<string> userNotify = null)
        {
            return new Task<IList<Category>>(() => new List<Category>() { new Category() });
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
                    if(ResponseHTMLBody.IndexOf("<a href=\"/",0, true) == -1)
                    {
                        Debug.Write("Task<AdvertSearchResult> GetAdvertListAsync => url not found");
                        yield break;
                    }
                    ResponseHTMLBody.CutFoward("<a href=\"/");
                    string aUrl = ResponseHTMLBody.ToString(0, ResponseHTMLBody.IndexOf("\">", 0, true)).Trim();
                    if(aUrl.Length == 0)
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
            var responseString =  await GetWebPage(urlRequest, userNotify);
            if(responseString.Length == 0)
            {
                resultList.ErrorMessage = "Sprawdź połączenie internetowe i spróbuj ponownie.";
                responseString.Clear();
                return resultList;
            }
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

            // image sid
            string aSsid = String.Empty;
            if (BodyResult.IndexOf("rel=\"" + advertShort.AdverIDinRzeszowiak + "|", 0, true) != -1)
            {
                BodyResult.CutFoward("rel=\"" + advertShort.AdverIDinRzeszowiak + "|" );
                aSsid = BodyResult.ToString(0, BodyResult.IndexOf("\"", 0, true)).Trim();
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
                PhoneImage = (aSsid?.Length > 0) ? new RzeszowiakImageContainer(aSsid, advertShort.AdverIDinRzeszowiak, advertShort.URLPath) : null,
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
                    using (HttpResponseMessage response = await client.GetAsync(Url).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !response.IsSuccessStatusCode");
                            userNotify?.Invoke("Niepoprawna odpowiedź z serwera");
                            return BodyString;
                        }
                        Debug.Write("Response: " + response.Headers.ToString);
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

    public class RzeszowiakImageContainer
    {
        public string Session { get; private set; }
        public ImageSource ImageData { get; private set; }
        public event EventHandler<OnDownloadFinishEvenArgs> OnDownloadFinish;

        private string _ssid = String.Empty;
        private string _advertURL = String.Empty;
        private int _adrverID = 0;

        public RzeszowiakImageContainer(string ssid, int advertID, string advertURL)
        {
            Session = new Guid().ToString();
            if (ssid == null || advertID == 0)
            {
                Debug.Write("RzeszowiakImageContainer => ssid == null || advertID == 0");
                return;
            }

            _ssid = ssid;
            _adrverID = advertID;
            _advertURL = advertURL;
        }

        public async Task<bool> DownloadImage()
        {
            if (_ssid == null || _adrverID == 0)
            {
                Debug.Write("RzeszowiakImageContainer => DownloadImage()");
                return false;
            }

            var inputData = new Dictionary<string, string>()
            {
                { "oid", _adrverID.ToString() },
                { "ssid", _ssid },
            };

            var BodyResult = await PostWebPage("http://www.rzeszowiak.pl/telefon/", inputData, _advertURL);
            if (BodyResult.Length == 0)
            {
                Debug.Write("GetAdvertAsync => BodyResult.Length == 0");
                return false;
            }
            Debug.Write("Image Data source: " + BodyResult.ToString());
            ImageData = Base64ToImage(BodyResult.ToString());
            BodyResult.Clear();
            OnDownloadFinish?.Invoke(this, new OnDownloadFinishEvenArgs(_ssid));
            return true;
        }

        public ImageSource Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ImageSource image = ImageSource.FromStream(() => ms);
                return image;
            }
        }

        public void Dispose()
        {
            OnDownloadFinish = null;
        }

        protected static async Task<StringBuilder> PostWebPage(string Url, Dictionary<string, string> postData, string RefererUrl,  Action<string> userNotify = null)
        {
            StringBuilder BodyString = new StringBuilder();
            using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(6) })
            {
                try
                {
                    var keyValues = new List<KeyValuePair<string, string>>();
                    foreach (var item in postData)
                    {
                        Debug.Write($"key {item.Key} value {item.Value}");
                        keyValues.Add(new KeyValuePair<string, string>(item.Key, item.Value));
                    }
                    

                    var inputContent = new FormUrlEncodedContent(postData);
                    inputContent.Headers.Add("Referer", RefererUrl);

                    using (HttpResponseMessage response = await client.PostAsync(Url, inputContent).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !response.IsSuccessStatusCode");
                            userNotify?.Invoke("Niepoprawna odpowiedź z serwera");
                            return BodyString;
                        }
                        using (HttpContent content = response.Content)
                        {
                            var byteArray = await content.ReadAsStringAsync();
                            Debug.Write("PostWebPage => " + byteArray);
                            BodyString.Append(byteArray);
                        }
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    Debug.Write("PostWebPage => System.Threading.Tasks.TaskCanceledException");
                    userNotify?.Invoke("Błąd połączenia. Przekroczono limit połączenia");
                    return BodyString;
                }
                catch (Exception e)
                {
                    Debug.Write("PostWebPage => " + e.Message);
                    userNotify?.Invoke("Błąd połączenia z serwerem.");
                    return BodyString;
                }
            }
            return BodyString;
        }
    }

    public class OnDownloadFinishEvenArgs : EventArgs
    {
        public string Session { get; private set; }
        public OnDownloadFinishEvenArgs(string sessionSet)
        {
            Session = sessionSet;
        }
    }
}
