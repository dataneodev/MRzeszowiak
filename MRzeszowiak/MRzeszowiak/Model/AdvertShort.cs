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
        public string Title { get; set; } = String.Empty;
        public DateTime DateAddDateTime { get; set; }
        [Ignore]
        public string DateAddString { get => GetFormatedDateTime(DateAddDateTime); } 
        public int Price { get; set; }
        public bool Highlighted { get; set; }

        protected string uRLPath = String.Empty;
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
        public string DescriptionShort { get; set; } = String.Empty;
        protected string thumbnailUrl = String.Empty;
        protected string blankImage = "https://sites.google.com/site/dataneosoftware/polski/mrzeszowiak/no-image.png?attredirects=0";
        public string ThumbnailUrl
        {
            get { return ((thumbnailUrl?.Length??0) == 0) ? blankImage : RZESZOWIAK_BASE_URL + thumbnailUrl; }
            set
            {
                if(value?.IndexOf("/wsp/mini/l_no.gif") != -1 || value == blankImage)
                    thumbnailUrl = String.Empty;
                else
                    thumbnailUrl = value?.Replace(RZESZOWIAK_BASE_URL, String.Empty);
            }
        }
        [Ignore]
        public bool RowEven { get; set; }
        protected string GetFormatedDateTime(DateTime dt)
        {
            if(dt.Date == DateTime.Today)
                return String.Format("dziś, {0:HH:mm}", dt);

            if (dt.Date == DateTime.Now.AddDays(-1).Date)
                return String.Format("wczoraj, {0:HH:mm}", dt);

            if (dt.Date == DateTime.Now.AddDays(1).Date)
                return String.Format("jutro, {0:HH:mm}", dt);

            return String.Format("{0:yyyy-MM-dd HH:mm}", dt); ;
        }

        public override int GetHashCode()
        {
            return AdverIDinRzeszowiak;
        }

        public bool Equals(AdvertShort other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return AdverIDinRzeszowiak == other?.AdverIDinRzeszowiak;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as AdvertShort);
        }
    }
}
