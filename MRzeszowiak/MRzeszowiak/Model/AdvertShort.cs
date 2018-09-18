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
            get { return uRLPath; }
            set { uRLPath = value; }
        }

        public string URL
        {
            get { return URLPath.Length > 0 ? RZESZOWIAK_BASE_URL + URLPath : 
                        RZESZOWIAK_BASE_URL + "uslugi-transportowe-" + AdverIDinRzeszowiak.ToString();  }
            set { URLPath = value; }
        }
        public string DescriptionShort { get; set; }
        protected string thumbnailUrl;
        public string ThumbnailUrl
        {
            get
            {
                return (thumbnailUrl.IndexOf("/wsp/mini/l_no.gif") != -1) ? 
                        "https://sites.google.com/site/dataneosoftware/polski/mrzeszowiak/no-image.png?attredirects=0" : 
                        RZESZOWIAK_BASE_URL + thumbnailUrl;
            }
            set { thumbnailUrl = value; }
        }
        public bool RowEven { get; set; }
    }
}
