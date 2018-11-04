using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Model
{
    public class AdvertShort
    {
        protected const string RZESZOWIAK_BASE_URL = "http://www.rzeszowiak.pl/";
        [PrimaryKey, AutoIncrement]
        public int IdDb { get; set; }
        public int AdverIDinRzeszowiak { get; set; }
        [Ignore]
        public Category Category { get; set; }
        public string CategorySerialized
        {
            get => JsonConvert.SerializeObject(Category);
            set
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    Category = JsonConvert.DeserializeObject<Category>(value);
                    if (Category?.ChildCategory != null)
                        foreach (var child in Category.ChildCategory)
                            child.ParentCategory = Category;
                }
                catch (System.Exception e)
                {
                    Debug.Write("CategorySerialized error => " + e.Message);
                }
            }
        }
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
        [Ignore]
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
        [Ignore]
        public bool RowEven { get; set; }
    }
}
