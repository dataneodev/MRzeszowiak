using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class Category
    {
        public short Id {get; set; } 
        public short MasterId { get; set; }
        public string MasterName { get; set; }
        public string Title { get; set; }
        public short Views { get; set; }
        public string GETPath { get; set; } //ex. Dla-domu-Meble-281
        public IReadOnlyList<string> ChildCategory { get; set; }// ex. ciezarowe //http://www.rzeszowiak.pl/Motoryzacja-Sprzedam-3010011505?r=ciezarowe

        private int selectedChildCategory;
        public int SelectedChildCategory
        {
            get {

                return (selectedChildCategory < (ChildCategory?.Count ?? 0)) ? selectedChildCategory : 0;
            }
            set { selectedChildCategory = value; }
        }

    }
}
