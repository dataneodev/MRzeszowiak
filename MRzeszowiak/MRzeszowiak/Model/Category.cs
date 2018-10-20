using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class MasterCategory
    {
        [PrimaryKey, AutoIncrement]
        public int IdDB { get; set; }
        public short Id { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
    }

    public class Category : MasterCategory
    {
        [ManyToOne]
        public MasterCategory Master { get; set; } 
        public string GETPath { get; set; } //ex. Dla-domu-Meble-281 or 
        [OneToMany]
        public List<ChildCategory> ChildCategory { get; set; }
        private ChildCategory selectedChildCategory;
        [ManyToOne]
        public ChildCategory SelectedChildCategory
        {
            get { return ((selectedChildCategory != null) && (ChildCategory.IndexOf(selectedChildCategory) != -1)) ? selectedChildCategory : null; }
            set
            {
                if (value != null && ChildCategory.IndexOf(value) != -1)
                    selectedChildCategory = value;
                else
                    selectedChildCategory = null;
            }
        }
        [Ignore]
        public string GetFullTitle
        { get
            {
                string result = (Master != null) ? Master.Title + " - " + Title : Title;
                result += (SelectedChildCategory != null) ? " - " + SelectedChildCategory.Title : String.Empty;
                return result;
            }
        }
        [Ignore]
        public static string TitleForNull
        {
            get { return "Wszystkie kategorie"; }
        }

    }

    public class ChildCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
        [Ignore]
        public Category ParentCategory { get; set; }
    }


}
