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
        public bool Equals(MasterCategory other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Id == other.Id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as MasterCategory);
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public static bool operator ==(MasterCategory left, MasterCategory right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(MasterCategory left, MasterCategory right)
        {
            return !Equals(left, right);
        }
    }

    public class Category : MasterCategory
    {
        public MasterCategory Master { get; set; } 
        public string GETPath { get; set; } //ex. Dla-domu-Meble-281 or 
        public List<ChildCategory> ChildCategory { get; set; }
        private ChildCategory selectedChildCategory;
        public ChildCategory SelectedChildCategory
        {
            get { return selectedChildCategory; }
            set
            {   
            selectedChildCategory = null;
            if (value != null)
                foreach(var item in ChildCategory)
                    if(value == item)
                    {
                        selectedChildCategory = item;
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

        public bool Equals(Category other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return string.Equals(GETPath, other.GETPath);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Category);
        }
        public override int GetHashCode()
        {
            return GETPath?.GetHashCode() ?? 0;
        }
        public static bool operator ==(Category left, Category right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Category left, Category right)
        {
            return !Equals(left, right);
        }
    }

    public class ChildCategory
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
        [JsonIgnore]
        public Category ParentCategory { get; set; }

        public bool Equals(ChildCategory other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return string.Equals(ID, other.ID);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ChildCategory);
        }
        public override int GetHashCode()
        {
            return ID?.GetHashCode() ?? 0;
        }
        public static bool operator ==(ChildCategory left, ChildCategory right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(ChildCategory left, ChildCategory right)
        {
            return !Equals(left, right);
        }
    }


}
