
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

        protected List<MasterCategory> masterCategoryList = new List<MasterCategory>{
            new MasterCategory { Id = 12, Title = "Dla domu" },
            new MasterCategory { Id = 26, Title = "Dla dzieci" },
            new MasterCategory { Id = 16, Title = "Komputery" },
            new MasterCategory { Id = 1, Title = "Motoryzacja" },
            new MasterCategory { Id = 28, Title = "Nauka" },
            new MasterCategory { Id = 2, Title = "Nieruchomości" },
            new MasterCategory { Id = 25, Title = "Poznajmy się" },
            new MasterCategory { Id = 3, Title = "Praca" },
            new MasterCategory { Id = 34, Title = "Przemysł" },
            new MasterCategory { Id = 13, Title = "Różne" },
            new MasterCategory { Id = 30, Title = "Sport / Wypoczynek" },
            new MasterCategory { Id = 18, Title = "Ślub" },
            new MasterCategory { Id = 14, Title = "Telefony GSM" },
            new MasterCategory { Id = 10, Title = "Usługi" },
            new MasterCategory { Id = 35, Title = "Zapraszam na" },
            new MasterCategory { Id = 31, Title = "Zdrowie, uroda" },
            new MasterCategory { Id = 33, Title = "Zwierzęta" }
        };

        protected List<Category> categoryList = new List<Category>(); 

        public async Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false, Action<string> userNotify = null)
        {
            if (ForceReload == true)
            {
                var HttpResult = await GetWeb.GetWebPage("http://www.rzeszowiak.pl/kontakt/");
                if (!HttpResult.Success)
                {
                    Debug.Write("GetCategoryListAsync => !HttpResult.Success");
                    return categoryList;
                }

                if (HttpResult.BodyString.Length == 0)
                {
                    Debug.Write("GetCategoryListAsync => responseString.Length == 0");
                    return categoryList;
                }

                UpdateCategoryList(HttpResult.BodyString);
                HttpResult.BodyString.Clear();
            }
            return categoryList;
        }

        protected bool UpdateCategoryList(StringBuilder htmlBody)
        {
            Debug.Write("UpdateCategoryList");
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

            foreach(var cat in masterCategoryList)
                processMainCategory(htmlBody, cat);
            // end of main method
            // submethod sections
            void processMainCategory(StringBuilder html, MasterCategory mainCategory)
            {
                var search = $"<div class=\"menu-left-category\">{mainCategory.Title}</div>";
                var pos = html.IndexOf(search, 0, true);
                if (pos == -1)
                {
                    Debug.Write($"UpdateCategoryList => no menu {mainCategory.Title} found");
                    return;
                }
                html.Remove(0, pos);
                html.CutFoward("<ul class=\"menu-left-subcategories\">");

                search = "<br/>";
                var subcat = html.ToString(0, html.IndexOf(search, 0, true));
                Debug.Write("MainCategory: " + subcat);
                var submenu = subcat.Split(new string[] { "</li>" }, StringSplitOptions.RemoveEmptyEntries);

                if (submenu.Length > 0)
                    for (int i = 0; i < submenu.Length; i++)
                        if ((i + 1 < submenu.Length) && (submenu[i + 1].IndexOf("<ul class=\"subsubcategories\">") != -1))
                        {
                            var subMenu = String.Empty;
                            int b;
                            for(b = i + 1; b < submenu.Length; b++ )
                            {
                                if (submenu[b].IndexOf("</ul>") != -1) break;
                                subMenu += submenu[b];
                            }
                            processSubCategory(submenu[i], subMenu, mainCategory);
                            i = ++b;
                        }
                        else
                            processSubCategory(submenu[i], String.Empty, mainCategory);
            }

            void processSubCategory(string html, string submenu, MasterCategory mainCategory)
            {
                Debug.Write("Main html : " + html.Trim());
                if(submenu.Length > 0)
                    Debug.Write("Submenu : " + submenu);

                // path
                var search = "<a href=\"/";
                var pos = html.IndexOf(search);
                if (pos == -1)
                {
                    Debug.Write("processSubCategory => no path found");
                    return;
                }
                
                var path = html.GetItem(search, "\">").Trim();
                if(path.Length == 0)
                {
                    Debug.Write("processSubCategory => path length == 0");
                    return;
                }
                html = html.CutFoward(search);
                // id
                var idsearch = path.LastIndexOf("-") + 1;
                if(idsearch == 0 || !short.TryParse(path.Substring(idsearch, path.Length - idsearch), out short id))
                {
                    Debug.Write("processSubCategory => id not found or convert fail");
                    return;
                }
                //name
                var name = html.GetItem("</span>", "<span class=\"ilosc\"").Trim();
                if(name.Length == 0)
                {
                    Debug.Write("processSubCategory => name not found");
                    return;
                }
                //count
                html = html.CutFoward("<span class=\"ilosc\"");
                if (!short.TryParse(html.GetItem(">", "</span>"), out short count))
                {
                    Debug.Write("processSubCategory => count not found or convert fail");
                    return;
                }

                var newCategory = new Category
                {
                    Id = id,
                    Title = name,
                    Views = count,
                    GETPath = path,
                    Master = mainCategory,
                };

                newCategory.ChildCategory = processChildcategory(submenu, newCategory);
                categoryList.Add(newCategory);

                Debug.Write($"process : name={name} path={path} id={id} count={count}");
            }

            List<ChildCategory> processChildcategory(string subhtml, Category category)
            {
                var resultList = new List<ChildCategory>();
                if ((subhtml?.Length ?? 0) == 0) return resultList;

                var submenu = subhtml.Split(new string[] { "<li>" }, StringSplitOptions.RemoveEmptyEntries);
                if (submenu.Length > 0)
                    foreach (var single in submenu)
                    {
                        var search = "<a href=\"/";
                        if (single.IndexOf(search) == -1) continue;
                        var id = single.GetItem("?r=", "\">");
                        var title = single.GetItem("</span>", "<span").Trim();
                        var countS = single.CutFoward("</span>");
                        countS = countS.GetItem("\"ilosc\">", "</span>");
                        if (!short.TryParse(countS, out short count))
                        {
                            Debug.Write("processChildcategory => count not found or convert fail");
                            return resultList;
                        }

                        Debug.Write($"processChildcategory : id={id} title={title} count={count}");
                    }
                        
                return resultList;
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
                    if(aAdd.Length == 0)
                    {
                        Debug.Write("Task<AdvertSearchResult> aAdd.Length == 0");
                        yield break;
                    }

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

            UpdateCategoryList(responseString); // update category

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

            UpdateCategoryList(BodyResult); // update category

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
