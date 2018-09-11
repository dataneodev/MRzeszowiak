using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertSearch
    {
        public int AdvertID { get; set; } // if set
        public string SearchPattern { get; set; } = String.Empty; // not empty
        public Category Category { get; set; } // null for all
        public AddType DateAdd { get; set; } = AddType.all;
        public SortType Sort { get; set; } = SortType.dateaddDesc;
        public int PriceMin { get; set; }
        public int PriceMax { get; set; }
    }

    public enum AddType
    {
        last24h,
        last3days,
        last7days,
        all
    }

    public enum SortType
    {
        dateadd = 1,
        dateaddDesc = 2, // najnowsze
        price = 4, // rosnąco
        priceDesc = 3

    }
}
