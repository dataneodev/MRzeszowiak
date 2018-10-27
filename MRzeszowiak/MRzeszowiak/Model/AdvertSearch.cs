using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertSearch
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";
        protected const sbyte RECORD_ON_PAGE = 25;
        public string SearchPattern { get; set; } = String.Empty; // not empty

        [ForeignKey(typeof(Category)), Indexed]
        public int CategoryId { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Category CategorySearch { get; set; } // null for all
        public AddType DateAdd { get; set; } = AddType.all;
        public SortType Sort { get; set; } = SortType.dateadd;
        public Nullable<int> PriceMin { get; set; }
        public Nullable<int> PriceMax { get; set; }
        public Nullable<int> RequestPage { get; set; }
        [Ignore]
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
                char GetURLDeliver(string url) => (url?.IndexOf('?') != -1) ? '&' : '?';
                
                //in page search
                if (CategorySearch != null)
                {
                    urlRequest = $"{RZESZOWIAK_BASE_URL}{CategorySearch.GETPath}{GetUrlParams(RequestPage ?? 1, DateAdd, Sort)}";
                    urlRequest += (CategorySearch.SelectedChildCategory != null) ? $"?r={CategorySearch.SelectedChildCategory.ID}" : String.Empty;
                    urlRequest += ((SearchPattern?.Length??0) > 0) ? $"{GetURLDeliver(urlRequest)}z={SearchPattern}" : String.Empty;
                    urlRequest += (PriceMin != null) ? $"{GetURLDeliver(urlRequest)}min={PriceMin}" : String.Empty;
                    urlRequest += (PriceMax != null) ? $"{GetURLDeliver(urlRequest)}max={PriceMax}" : String.Empty;
                }

                // advance search string
                if (SearchPattern.Length > 0 && CategorySearch == null)
                {
                    urlRequest = $"{RZESZOWIAK_BASE_URL}szukaj/?kat={CategorySearch?.Id ?? 0}&pkat=0&dodane={(int)DateAdd}&z={SearchPattern}";
                    urlRequest += (PriceMin != null) ? $"&min={PriceMin}" : String.Empty;
                    urlRequest += (PriceMax != null) ? $"&max={PriceMax}" : String.Empty;
                    urlRequest += (RequestPage ?? 0) > 0 ? $"&strona={RequestPage}" : String.Empty;
                }

                // last add 
                if (SearchPattern.Length == 0 && CategorySearch == null)
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
