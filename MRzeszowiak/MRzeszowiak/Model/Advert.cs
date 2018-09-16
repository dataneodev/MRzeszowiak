using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Model
{
    public class Advert
    {      
        //main
        public int AdverIDinRzeszowiak { get; set; }
        public Category Category { get; set; }
        public string Title { get; set; }
        public string DateAddString { get; set; }
        public string ExpiredString { get; set; }
        public int Views { get; set; }
        public int Price { get; set; }
        public bool Highlighted { get; set; }
        //description
        public string DescriptionHTML { get; set; }
        //phone image
        public Image PhoneImage { get; set; }
        //Additional data
        public Dictionary<string, string> AdditionalData { get; set; }
        //Image URLs
        public IList<string> ImageURLsList {get; set; }
        public bool IsFavorite { get; set; }
    }
}
