using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertSearch
    {
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";
        protected const sbyte RECORD_ON_PAGE = 25;
        public int AdvertID { get; set; } // if set
        public string SearchPattern { get; set; } = String.Empty; // not empty
        public MasterCategory MasterCategory { get; set; }
        public Category Category { get; set; } // null for all
        public AddType DateAdd { get; set; } = AddType.all;
        public SortType Sort { get; set; } = SortType.dateadd;
        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }
        public int? RequestPage { get; set; }
        public string GetURL
        {
            get
            {
                // preparing URL request
                string urlRequest = String.Empty;
                string GetUrlParams(int PageType, AddType addType, SortType sortType)
                {
                    string Page = String.Format("{0:D2}", (int)PageType);
                    return $"0{Page}{(int)sortType}{RECORD_ON_PAGE}{(int)addType}";
                }
                //select category without search
                if (SearchPattern.Length == 0 && AdvertID == 0 && Category != null)
                {
                    urlRequest = $"{RZESZOWIAK_BASE_URL}{Category.GETPath}{GetUrlParams(RequestPage ?? 1, DateAdd, Sort)}";
                    urlRequest += (Category.SelectedChildCategory != null) ? $"?r={Category.SelectedChildCategory.ID}" : String.Empty;
                }
                // search string
                if (SearchPattern.Length > 0)
                {
                    if (Category == null || MasterCategory == null)
                    {
                        urlRequest = $"{RZESZOWIAK_BASE_URL}szukaj/?kat={Category?.Id ?? 0}&pkat=0&dodane={Sort}&z={SearchPattern}";
                        urlRequest += (RequestPage ?? 0) > 0 ? $"&strona={RequestPage}" : String.Empty;
                    }        
                    else
                    {
                        urlRequest = $"{RZESZOWIAK_BASE_URL}{Category.GETPath}{GetUrlParams(RequestPage ?? 1, DateAdd, Sort)}?z={SearchPattern}";
                        urlRequest += (PriceMin != null) ? $"&min={PriceMin}" : String.Empty;
                        urlRequest += (PriceMax != null) ? $"&max={PriceMax}" : String.Empty;                        
                    }
                }
                // search id
                //if (AdvertID != 0)
                //    urlRequest = $"{RZESZOWIAK_BASE_URL}szukaj/numer:{AdvertID}";
                // last add 
                if (SearchPattern.Length == 0 && AdvertID == 0 && Category == null)
                {
                    urlRequest = RZESZOWIAK_BASE_URL;
                    urlRequest += (RequestPage ?? 0) > 0 ? $"?start={RequestPage}" : String.Empty;
                }
                    
                Debug.Write($"AdvertSearch => GetURL => {urlRequest}");
                return urlRequest;
            }
        }
        
    }

    public enum AddType
    {
        last24h = 1,
        last3days = 2,
        last7days = 3,
        last14days = 4,
        all = 5
    }

    public enum SortType
    {
        dateadd = 1, // najnowsze
        dateaddDesc = 2, 
        price = 4, // rosnąco
        priceDesc = 3
    }
}
