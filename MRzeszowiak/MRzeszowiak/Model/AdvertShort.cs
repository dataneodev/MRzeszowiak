using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Model
{
    public class AdvertShort
    {
        public int AdverIDinRzeszowiak { get; set; }
        public byte Category { get; set; }
        public string Title { get; set; }
        public string DateAddString { get; set; }
        public int Price { get; set; }
        public bool Highlighted { get; set; }
        public string DescriptionShort { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool RowEven { get; set; }
    }
}
