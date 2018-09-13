using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Extends
{
    public static class StringExtensions
    {
        public static string CutFoward(this string ciag, string search)
        {
            if (search.Length == 0) { return ciag; }
            int pos = ciag.IndexOf(search);
            if (pos == -1) { return ciag; }
            pos += search.Length;
            return ciag.Substring(pos);
        }

        public static string CutBacking(this string ciag, string search)
        {
            if (search.Length == 0) { return ciag; }
            int pos = ciag.IndexOf(search);
            if (pos == -1) { return ciag; }
            return ciag.Substring(0, pos);
        }

        public static string GetItem(this string ciag, string posStart, string posEnd)
        {
            if (posStart.Length == 0) { return ciag; }
            if (posEnd.Length == 0) { return ciag; }

            int pos = ciag.IndexOf(posStart);
            if (pos == -1) { return ciag; }
            pos += posStart.Length;

            int pos2 = ciag.IndexOf(posEnd);
            if (pos2 == -1 || pos2 < pos) { return ciag; }
            return ciag.Substring(pos, pos2 - pos);
        }

        public static string StripHTML(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
