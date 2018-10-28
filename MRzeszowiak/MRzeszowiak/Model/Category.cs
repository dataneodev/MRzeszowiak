using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class MasterCategory
    {
        public short Id { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
    }

    public class Category : MasterCategory
    {
        public MasterCategory Master { get; set; } 
        public string GETPath { get; set; } //ex. Dla-domu-Meble-281 or 
        public List<ChildCategory> ChildCategory { get; set; }
        private ChildCategory selectedChildCategory;
        public ChildCategory SelectedChildCategory
        {
            get { return selectedChildCategory != null ? selectedChildCategory : null; }
            set
            {   
            selectedChildCategory = null;
            if (value != null)
                foreach(var item in ChildCategory)
                    if(value.ID == item.ID)
                    {
                        selectedChildCategory = value;
                        break;
                    }        
            }
        }
        [JsonIgnore]
        public string GetFullTitle
        { get
            {
                string result = (Master != null) ? Master.Title + " - " + Title : Title;
                result += (SelectedChildCategory != null) ? " - " + SelectedChildCategory.Title : String.Empty;
                return result;
            }
        }
        [JsonIgnore]
        public static string TitleForNull
        {
            get { return "Wszystkie kategorie"; }
        }

    }

    public class ChildCategory
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
        [JsonIgnore]
        public Category ParentCategory { get; set; }
    }


}
