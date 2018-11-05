using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;

namespace MRzeszowiak.Model
{
    public class Advert : AdvertShort
    {      
        public DateTime ExpiredDatetime { get; set; }
        [Ignore]
        public string ExpiredString { get => GetFormatedDateTime(ExpiredDatetime); }
        public int Views { get; set; }
        public string DescriptionHTML { get; set; } = String.Empty;
        public string PhoneSsid { get; set; } = String.Empty;
        [Ignore]
        public Cookie PhonePHPSSESION { get; set; }
        [Ignore]
        public Dictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();
        [Ignore]
        public List<string> ImageURLsList { get; set; } = new List<string>();
        public DateTime VisitPageDate { get; set; }
        public string EmailToken { get; set; } = String.Empty;
        public byte[] PhoneImageByteArray { get; set; }
        // for db
        public string AdditionalDataSerialize
        {
            get => GetSerialized<Dictionary<string, string>>(AdditionalData);
            set => AdditionalData = SetSerialized<Dictionary<string, string>>(value);
        }
        public string ImageURLsListSerialize
        {
            get => GetSerialized<List<string>>(ImageURLsList);
            set => ImageURLsList = SetSerialized<List<string>>(value);
        }

        string GetSerialized<T>(T objToSerialie)
        {
            return JsonConvert.SerializeObject(objToSerialie);
        }

        T SetSerialized<T>(string objToDeserialize)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            try
            {
                return JsonConvert.DeserializeObject<T>(objToDeserialize);
            }
            catch (System.Exception e)
            {
                Debug.Write("AutostartAdvertSearchSerialized error => " + e.Message);
            }
            return default(T);
        }

        public bool Equals(Advert other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return AdverIDinRzeszowiak == other?.AdverIDinRzeszowiak;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Advert);
        }
    }
}
