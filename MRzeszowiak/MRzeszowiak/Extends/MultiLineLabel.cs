using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Extends
{
    public class MultiLineLabel : Label
    {
        // Default number of lines is 1 so MultiLineLabel behaves like a standard Label if Lines is not set
        public int Lines { get; set; } = 4;
    }
}
