using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertSearchResult
    {
        public bool Correct { get; set; }
        public string ErrorMessage { get; set; }
        public int Page { get; set; }
        public int AllPage { get; set; }
        public List<AdvertShort> AdvertShortsList { get; set; }
    }
}
