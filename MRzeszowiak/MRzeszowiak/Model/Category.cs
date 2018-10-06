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
            get { return ((selectedChildCategory != null) && (ChildCategory.IndexOf(selectedChildCategory) != -1)) ? selectedChildCategory : null; }
            set
            {
                if (value != null && ChildCategory.IndexOf(value) != -1)
                    selectedChildCategory = value;
                else
                    selectedChildCategory = null;
            }
        }
    }

    public class ChildCategory
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
        public Category ParentCategory { get; set; }
    }


}
