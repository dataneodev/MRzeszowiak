using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class Category
    {
        public short Id {get; set; } 
        public string Title { get; set; }
        public short MasterId { get; set; } // if 0 then its master
        public string MasterTitle { get; set; }
        public short Views { get; set; }
        public string GETPath { get; set; } //ex. Dla-domu-Meble-281
        public List<string> ChildCategory { get; set; }// ex. ciezarowe //http://www.rzeszowiak.pl/Motoryzacja-Sprzedam-3010011505?r=ciezarowe

        private string selectedChildCategory;
        public string SelectedChildCategory
        {
            get { return ((selectedChildCategory != null) && (ChildCategory.IndexOf(selectedChildCategory) != -1)) ? selectedChildCategory : null; }
            set
            {
                if(value != null && ChildCategory.IndexOf(value) != -1)
                    selectedChildCategory = value;
                else
                    selectedChildCategory = null;                
            }
        }

    }
}
