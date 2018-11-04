using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertSerialized : Advert
    {
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
    }
}
