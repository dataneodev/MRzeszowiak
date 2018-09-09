using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertSearch
    {
        public int AdvertID { get; set; } // if set
        public string SearchPattern { get; set; }  // not empty
        public Category Category { get; set; } // null for all
        public AddType DateAdd { get; set; } = AddType.all;
    }

    public enum AddType
    {
        last24h,
        last3days,
        last7days,
        all
    }
}
