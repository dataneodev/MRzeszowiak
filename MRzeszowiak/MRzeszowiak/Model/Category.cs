using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    class Category
    {
        public short Id {get; set; } // 0 - no advert
        public Category ParentCategory { get; set; } // null - no parent
        public string Title { get; set; }
        public short Views { get; set; }
    }
}
