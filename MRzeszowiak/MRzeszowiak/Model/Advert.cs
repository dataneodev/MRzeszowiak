﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MRzeszowiak.Model
{
    public class Advert : AdvertShort
    {      
        public string ExpiredString { get; set; } = String.Empty;
        public int Views { get; set; }
        public string DescriptionHTML { get; set; } = String.Empty;
        public string PhoneSsid { get; set; } = String.Empty;
        [Ignore]
        public Cookie PhonePHPSSESION { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public Dictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<string> ImageURLsList { get; set; } = new List<string>();
        public DateTime VisitPageDate { get; set; }
        public string EmailToken { get; set; }
    }
}
