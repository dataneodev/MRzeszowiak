
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
    public class RzeszowiakRepository : IRzeszowiak
    {
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";

        protected static List<MasterCategory> masterCategoryList = new List<MasterCategory>{
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

        protected static List<Category> categoryList = getCategoryList();

        protected static List<Category> getCategoryList()
        {
            var r = new List<Category>();
            // home 
            var masterCategory = masterCategoryList[0];
            r.Add(nCat(281, "Meble", "Dla-domu-Meble-281", masterCategory, null));
            r.Add(nCat(280, "RTV, AGD", "Dla-domu-RTV--AGD-280", masterCategory, null));
            r.Add(nCat(282, "Mat. budowlane", "Dla-domu-Mat-budowlane-282", masterCategory, null));
            r.Add(nCat(101, "Do ogrodu", "Dla-domu-Do-ogrodu-101", masterCategory, null));
            r.Add(nCat(156, "Rośliny", "Dla-domu-Rosliny-156", masterCategory, null));
            r.Add(nCat(283, "Inne", "Dla-domu-Inne-283", masterCategory, null));
            //dla dzieci
            masterCategory = masterCategoryList[1];
            r.Add(nCat(274, "Ubranka", "Dla-dzieci-Ubranka-274", masterCategory, null));
            r.Add(nCat(278, "Opiekunki", "Dla-dzieci-Opiekunki-278", masterCategory, null));
            r.Add(nCat(293, "Przedszkola,żłobki", "Dla-dzieci-Przedszkola-zlobki-293", masterCategory, null));
            r.Add(nCat(275, "Zabawki", "Dla-dzieci-Zabawki-275", masterCategory, null));
            r.Add(nCat(276, "Pielęgnacja", "Dla-dzieci-Pielegnacja-276", masterCategory, null));
            r.Add(nCat(277, "Inne", "Dla-dzieci-Inne-277", masterCategory, null));
            //Komputery
            masterCategory = masterCategoryList[2];
            r.Add(nCat(248, "Sprzedam", "Komputery-Sprzedam-248", masterCategory, null));
            r.Add(nCat(249, "Kupię", "Komputery-Kupie-249", masterCategory, null));
            r.Add(nCat(286, "Akcesoria", "Komputery-Akcesoria-286", masterCategory, null));
            r.Add(nCat(152, "Tablety", "Komputery-Tablety-152", masterCategory, null));
            r.Add(nCat(153, "Konsole gier", "Komputery-Konsole-gier-153", masterCategory, null));
            r.Add(nCat(285, "Oprogramowanie", "Komputery-Oprogramowanie-285", masterCategory, null));
            r.Add(nCat(291, "Usługi", "Komputery-Uslugi-291", masterCategory, null));
            //Motoryzacja
            masterCategory = masterCategoryList[3];
            var cat = nCat(301, "Sprzedam", "Motoryzacja-Sprzedam-301", masterCategory, null);
            var child = new List<ChildCategory>
            {
                new ChildCategory(){ID = "samochody", Title = "Samochody", ParentCategory = cat },
                new ChildCategory(){ID = "motocykle", Title = "Motocykle", ParentCategory = cat },
                new ChildCategory(){ID = "skutery,quady", Title = "Skutery, quady", ParentCategory = cat },
                new ChildCategory(){ID = "przyczepy", Title = "Przyczepy", ParentCategory = cat },
                new ChildCategory(){ID = "dostawcze", Title = "Dostawcze", ParentCategory = cat },
                new ChildCategory(){ID = "ciezarowe", Title = "Cięzarowe ", ParentCategory = cat },
                new ChildCategory(){ID = "rolnicze", Title = "Rolnicze", ParentCategory = cat },
                new ChildCategory(){ID = "inne-moto", Title = "Inne", ParentCategory = cat },
            };
            cat.ChildCategory = child;
            r.Add(cat);
            r.Add(nCat(305, "Kupię", "Motoryzacja-Kupie-305", masterCategory, null));
            r.Add(nCat(236, "Zamienię", "Motoryzacja-Zamienie-236", masterCategory, null));
            r.Add(nCat(269, "Części, Akcesoria", "Motoryzacja-Czesci--Akcesoria-269", masterCategory, null));
            r.Add(nCat(299, "Opony,felgi,koła", "Motoryzacja-Opony-felgi-kola-299", masterCategory, null));
            r.Add(nCat(270, "Usługi", "Motoryzacja-Uslugi-270", masterCategory, null));
            //Nauka
            masterCategory = masterCategoryList[4];
            r.Add(nCat(245, "Podręczniki", "Nauka-Podreczniki-245", masterCategory, null));
            r.Add(nCat(266, "Korepetycje", "Nauka-Korepetycje-266", masterCategory, null));
            r.Add(nCat(294, "Języki obce", "Nauka-Jezyki-obce-294", masterCategory, null));
            r.Add(nCat(295, "Kursy,szkolenia", "Nauka-Kursy-szkolenia-295", masterCategory, null));
            r.Add(nCat(143, "Prezentacje maturalne", "Nauka-Prezentacje-maturalne-143", masterCategory, null));
            //Nieruchomości
            masterCategory = masterCategoryList[5];
            cat = nCat(307, "Sprzedam", "Nieruchomosci-Sprzedam-307", masterCategory, null);
            child = new List<ChildCategory>
            {
                new ChildCategory(){ID = "mieszkania", Title = "Mieszkania", ParentCategory = cat },
                new ChildCategory(){ID = "domy", Title = "Domy", ParentCategory = cat },
                new ChildCategory(){ID = "dzialki", Title = "Działki", ParentCategory = cat },
                new ChildCategory(){ID = "lokale", Title = "Lokale, garaże", ParentCategory = cat },
            };
            cat.ChildCategory = child;
            r.Add(cat);
            cat = nCat(258, "Sprzedam (A-agencje)", "Nieruchomosci-Sprzedam-agencje-258", masterCategory, null);
            child = new List<ChildCategory>
            {
                new ChildCategory(){ID = "mieszkania", Title = "Mieszkania", ParentCategory = cat },
                new ChildCategory(){ID = "domy", Title = "Domy", ParentCategory = cat },
                new ChildCategory(){ID = "dzialki", Title = "Działki", ParentCategory = cat },
                new ChildCategory(){ID = "lokale", Title = "Lokale, garaże", ParentCategory = cat },
            };
            cat.ChildCategory = child;
            r.Add(cat);
            r.Add(nCat(308, "Kupię", "Nieruchomosci-Kupie-308", masterCategory, null));
            r.Add(nCat(259, "Kupię (agencje)", "Nieruchomosci-Kupie-agencje-259", masterCategory, null));
            cat = nCat(210, "Mam do wynajęcia", "Nieruchomosci-Mam-do-wynajecia-210", masterCategory, null);
            child = new List<ChildCategory>
            {
                new ChildCategory(){ID = "pokoj", Title = "Pokój", ParentCategory = cat },
                new ChildCategory(){ID = "mieszkanie", Title = "Mieszkanie", ParentCategory = cat },
                new ChildCategory(){ID = "dom", Title = "Dom", ParentCategory = cat },
                new ChildCategory(){ID = "lokal", Title = "Lokal, garaż, hala", ParentCategory = cat },
                new ChildCategory(){ID = "inne-wynajem", Title = "Inne", ParentCategory = cat },
            };
            cat.ChildCategory = child;
            r.Add(cat);
            r.Add(nCat(218, "Szukam do wynajęcia", "Nieruchomosci-Szukam-do-wynajecia-218", masterCategory, null));
            r.Add(nCat(123, "Szukam do wynajęcia (A)", "Nieruchomosci-Szukam-do-wynajecia-agencje--123", masterCategory, null));
            r.Add(nCat(125, "Szukam współlokatora(ki)", "Nieruchomosci-Szukam-wspollokatoraki-125", masterCategory, null));
            r.Add(nCat(217, "Zamienię", "Nieruchomosci-Zamienie-217", masterCategory, null));
            //Poznajmy się
            masterCategory = masterCategoryList[6];
            r.Add(nCat(267, "Pani pozna ...", "Poznajmy-sie-Pani-pozna--267", masterCategory, null));
            r.Add(nCat(268, "Pan pozna ...", "Poznajmy-sie-Pan-pozna--268", masterCategory, null));
            //Praca
            masterCategory = masterCategoryList[7];
            r.Add(nCat(304, "Zatrudnię", "Praca-Zatrudnie-304", masterCategory, null));
            r.Add(nCat(140, "Zatrudnię - dodatkowa", "Praca-Zatrudnie---dodatkowa-140", masterCategory, null));
            r.Add(nCat(141, "Sprzedaż bezpośrednia", "Praca-Sprzedaz-bezposrednia-141", masterCategory, null));
            r.Add(nCat(142, "Praca przez internet", "Praca-Praca-przez-internet-142", masterCategory, null));
            r.Add(nCat(145, "Za granicą", "Praca-Za-granica-145", masterCategory, null));
            r.Add(nCat(211, "Poszukuję", "Praca-Poszukuje-211", masterCategory, null));
            r.Add(nCat(296, "Kursy, szkolenia", "Praca-Kursy--szkolenia-296", masterCategory, null));
            //Przemysł
            masterCategory = masterCategoryList[8];
            r.Add(nCat(135, "Elektronarzędzia", "Przemysl-Elektronarzedzia-135", masterCategory, null));
            r.Add(nCat(134, "Maszyny i urządzenia", "Przemysl-Maszyny-i-urzadzenia-134", masterCategory, null));
            r.Add(nCat(136, "Pozostałe", "Przemysl-Pozostale-136", masterCategory, null));
            //Różne
            masterCategory = masterCategoryList[9];
            r.Add(nCat(228, "Sprzedam", "Rozne-Sprzedam-228", masterCategory, null));
            r.Add(nCat(144, "Wspólne przejazdy", "Rozne-Wspolne-przejazdy-144", masterCategory, null));
            r.Add(nCat(229, "Kupię", "Rozne-Kupie-229", masterCategory, null));
            r.Add(nCat(235, "Oddam za ...", "Rozne-Oddam-za--235", masterCategory, null));
            r.Add(nCat(132, "Oddam za darmo", "Rozne-Oddam-za-darmo-132", masterCategory, null));
            r.Add(nCat(124, "Przyjmę za ...", "Rozne-Przyjme-za--124", masterCategory, null));
            r.Add(nCat(157, "Antyki,Starocie", "Rozne-Antyki-Starocie-157", masterCategory, null));
            r.Add(nCat(231, "Poszukuję", "Rozne-Poszukuje-231", masterCategory, null));
            r.Add(nCat(257, "Zdrowie", "Rozne-Zdrowie-257", masterCategory, null));
            r.Add(nCat(284, "Zgubiono, Znaleziono", "Rozne-Zgubiono--Znaleziono-284", masterCategory, null));
            r.Add(nCat(230, "Ubrania, obuwie", "Rozne-Ubrania--obuwie-230", masterCategory, null));
            //Sport / Wypoczynek
            masterCategory = masterCategoryList[10];
            r.Add(nCat(108, "Na sportowo", "Sport--Wypoczynek-Na-sportowo-108", masterCategory, null));
            r.Add(nCat(137, "Sprzęt sportowy - zima", "Sport--Wypoczynek-Sprzet-sportowy---zima-137", masterCategory, null));
            r.Add(nCat(138, "Sprzęt sportowy - lato", "Sport--Wypoczynek-Sprzet-sportowy---lato-138", masterCategory, null));
            r.Add(nCat(155, "Jeździectwo", "Sport--Wypoczynek-Jezdziectwo-155", masterCategory, null));
            r.Add(nCat(110, "Agroturystyka", "Sport--Wypoczynek-Agroturystyka-110", masterCategory, null));
            r.Add(nCat(111, "Kwatery prywatne", "Sport--Wypoczynek-Kwatery-prywatne-111", masterCategory, null));
            r.Add(nCat(112, "Ośrodki wczasowe", "Sport--Wypoczynek-Osrodki-wczasowe-112", masterCategory, null));
            r.Add(nCat(113, "Imprezy turystyczne", "Sport--Wypoczynek-Imprezy-turystyczne-113", masterCategory, null));
            r.Add(nCat(158, "Wspólny wypoczynek", "Sport--Wypoczynek-Wspolny-wypoczynek-158", masterCategory, null));
            //Ślub
            masterCategory = masterCategoryList[11];
            r.Add(nCat(254, "Suknie ślubne", "Slub-Suknie-slubne-254", masterCategory, null));
            r.Add(nCat(102, "Garnitury", "Slub-Garnitury-102", masterCategory, null));
            r.Add(nCat(103, "Obuwie", "Slub-Obuwie-103", masterCategory, null));
            r.Add(nCat(107, "Inna odzież", "Slub-Inna-odziez-107", masterCategory, null));
            r.Add(nCat(104, "Lokale, sale", "Slub-Lokale--sale-104", masterCategory, null));
            r.Add(nCat(106, "Dekoracje, ozdoby", "Slub-Dekoracje--ozdoby-106", masterCategory, null));
            r.Add(nCat(105, "Prezenty", "Slub-Prezenty-105", masterCategory, null));
            r.Add(nCat(255, "Usługi", "Slub-Uslugi-255", masterCategory, null));
            //Telefony GSM
            masterCategory = masterCategoryList[12];
            r.Add(nCat(239, "Sprzedam", "Telefony-GSM-Sprzedam-239", masterCategory, null));
            r.Add(nCat(237, "Kupię", "Telefony-GSM-Kupie-237", masterCategory, null));
            r.Add(nCat(256, "Akcesoria", "Telefony-GSM-Akcesoria-256", masterCategory, null));
            r.Add(nCat(240, "Uszkodzone", "Telefony-GSM-Uszkodzone-240", masterCategory, null));
            //Usługi
            masterCategory = masterCategoryList[13];
            r.Add(nCat(219, "Oferuję", "Uslugi-Oferuje-219", masterCategory, null));
            r.Add(nCat(220, "Poszukuję", "Uslugi-Poszukuje-220", masterCategory, null));
            r.Add(nCat(273, "Budowlane, Remonty", "Uslugi-Budowlane--Remonty-273", masterCategory, null));
            r.Add(nCat(290, "Transportowe", "Uslugi-Transportowe-290", masterCategory, null));
            r.Add(nCat(292, "Finansowe", "Uslugi-Finansowe-292", masterCategory, null));
            //Zapraszam na
            masterCategory = masterCategoryList[14];
            r.Add(nCat(148, "Dni Otwarte", "Zapraszam-na-Dni-Otwarte-148", masterCategory, null));
            r.Add(nCat(149, "Wykłady", "Zapraszam-na-Wyklady-149", masterCategory, null));
            r.Add(nCat(150, "Konferencje", "Zapraszam-na-Konferencje-150", masterCategory, null));
            r.Add(nCat(151, "Pozostałe", "Zapraszam-na-Pozostale-151", masterCategory, null));
            //Zdrowie, uroda
            masterCategory = masterCategoryList[15];
            r.Add(nCat(154, "Biżuteria", "Zdrowie--uroda-Bizuteria-154", masterCategory, null));
            r.Add(nCat(114, "Masaże", "Zdrowie--uroda-Masaze-114", masterCategory, null));
            r.Add(nCat(109, "Fitness, gimnastyka", "Zdrowie--uroda-Fitness--gimnastyka-109", masterCategory, null));
            r.Add(nCat(116, "Basen", "Zdrowie--uroda-Basen-116", masterCategory, null));
            r.Add(nCat(120, "Sprzęt, akcesoria", "Zdrowie--uroda-Sprzet--akcesoria-120", masterCategory, null));
            r.Add(nCat(121, "Dieta, odżywki", "Zdrowie--uroda-Dieta--odzywki-121", masterCategory, null));
            r.Add(nCat(115, "Kosmetyki", "Zdrowie--uroda-Kosmetyki-115", masterCategory, null));
            r.Add(nCat(122, "Med. alternatywna", "Zdrowie--uroda-Med-alternatywna-122", masterCategory, null));
            r.Add(nCat(117, "Usługi kosmetyczne", "Zdrowie--uroda-Uslugi-kosmetyczne-117", masterCategory, null));
            r.Add(nCat(139, "Stomatologia", "Zdrowie--uroda-Stomatologia-139", masterCategory, null));
            r.Add(nCat(118, "Usługi fryzjerskie", "Zdrowie--uroda-Uslugi-fryzjerskie-118", masterCategory, null));
            r.Add(nCat(119, "Pozostałe", "Zdrowie--uroda-Pozostale-119", masterCategory, null));
            //Zwierzęta
            masterCategory = masterCategoryList[16];
            r.Add(nCat(127, "Akcesoria", "Zwierzeta-Akcesoria-127", masterCategory, null));
            r.Add(nCat(133, "Akwarystyka", "Zwierzeta-Akwarystyka-133", masterCategory, null));
            r.Add(nCat(128, "Psy i koty z rodowodem", "Zwierzeta-Psy-i-koty-z-rodowodem-128", masterCategory, null));
            r.Add(nCat(129, "Psy i koty za darmo", "Zwierzeta-Psy-i-koty-za-darmo-129", masterCategory, null));
            r.Add(nCat(147, "Hodowlane", "Zwierzeta-Hodowlane-147", masterCategory, null));
            r.Add(nCat(130, "Pozostałe zwierzęta", "Zwierzeta-Pozostale-zwierzeta-130", masterCategory, null));
            r.Add(nCat(131, "Usługi", "Zwierzeta-Uslugi-131", masterCategory, null));

            return r;

            Category nCat(short id, string title, string path, MasterCategory master, List<ChildCategory> childList)
            {
                return new Category()
                {
                    Id = id,
                    Title = title,
                    GETPath = path,
                    Master = master,
                    ChildCategory = childList
                };
            }
        }

        public async Task<IList<MasterCategory>> GetMasterCategoryListAsync(bool ForceReload = false)
        {
            if (ForceReload == true)
                await ForceUpdate();
            return masterCategoryList;
        }

        public async Task<IList<Category>> GetCategoryListAsync(bool ForceReload = false)
        {
            if (ForceReload == true)
                await ForceUpdate();
            return categoryList;
        }

        public async Task<bool> SendUserMessage(Advert advert, String message)
        {
            if(advert == null && message?.Length == 0)
            {
                return false;
            }
            return true;
        }

        protected async Task ForceUpdate()
        {
            Debug.Write("ForeceUpdate");
            var HttpResult = await GetWeb.GetWebPage("http://www.rzeszowiak.pl/kontakt/");
            if (!HttpResult.Success)
            {
                Debug.Write("GetCategoryListAsync => !HttpResult.Success");
                return;
            }

            if (HttpResult.BodyString.Length == 0)
            {
                Debug.Write("GetCategoryListAsync => responseString.Length == 0");
                return;
            }

            UpdateCategoryList(HttpResult.BodyString);
            HttpResult.BodyString.Clear();
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

            foreach(var cat in categoryList)
            {
                if (!processPath(htmlBody, cat.GETPath, out short views)) return false;
                cat.Views = views;

                if ((cat.ChildCategory?.Count??0) > 0)
                    foreach (var child in cat.ChildCategory)
                    {
                        if (!processPath(htmlBody, $"{cat.GETPath}?r={child.ID}", out views)) return false;
                        child.Views = views;
                    } 
            }
            
            // update main category views
            foreach(var mcar in masterCategoryList)
            {
                var listCat = categoryList.FindAll((cat) => { if (cat.Master.Id == mcar.Id){ return true; } else { return false; } });
                short sumViews = 0;
                foreach (var item in listCat)
                    sumViews += item.Views;
                mcar.Views = sumViews;
            }       
                    
            bool processPath(StringBuilder html, string categoryPath, out short views)
            {
                views = 0;
                bool cutSB(string cutPatter, string debugMsg)
                {
                    var pos = html.IndexOf(cutPatter, 0, true);
                    if (pos == -1)
                    {
                        Debug.Write(debugMsg);
                        return false;
                    }
                    pos += cutPatter.Length;
                    html.Remove(0, pos);
                    return true;
                };

                if (!cutSB($"href=\"/{categoryPath}\">", $"UpdateCategoryList => no menu {categoryPath} found")) return false;
                if (!cutSB("class=\"ilosc\">", "UpdateCategoryList => no count found")) return false;

                if(! short.TryParse(html.ToString(0, html.IndexOf("</span>", 0, true)), out views))
                {
                    Debug.Write("UpdateCategoryList => count convert fail");
                    return false;
                }
                return true;
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
                        Category = searchParams.CategorySearch,
                        Highlighted = highlighted,
                        RowEven = rowEven
                    };
                    rowEven = !rowEven;
                } while (promoPos != -1 || normalPos != -1);
            }

            var HttpResult = await GetWeb.GetWebPage(urlRequest);
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
