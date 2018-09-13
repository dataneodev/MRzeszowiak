using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Model
{
    public class AdvertShort
    {
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";
        public int AdverIDinRzeszowiak { get; set; }
        public Category Category { get; set; }
        public string Title { get; set; }
        public string DateAddString { get; set; }
        public int Price { get; set; }
        public bool Highlighted { get; set; }
        protected string uRLPath;
        public string URLPath
        {
            get { return RZESZOWIAK_BASE_URL + uRLPath;  }
            set { uRLPath = value; }
        }
        public string DescriptionShort { get; set; }
        protected string thumbnailUrl;
        public string ThumbnailUrl
        {
            get { return RZESZOWIAK_BASE_URL + thumbnailUrl; }
            set { thumbnailUrl = value; }
        }
        public bool RowEven { get; set; }
    }
}
