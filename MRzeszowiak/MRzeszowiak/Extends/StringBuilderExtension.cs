using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Extends
{
    public static class StringBuilderExtension
    {
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        public static StringBuilder CutFoward(this StringBuilder ciag, string search)
        {
            if (search.Length == 0) { return ciag; }
            int pos = ciag.IndexOf(search, 0, true);
            if (pos == -1) { return ciag; }
            pos += search.Length ;
            return ciag.Remove(0, pos);
        }

        public static StringBuilder CutBacking(this StringBuilder ciag, string search)
        {
            if (search.Length == 0) { return ciag; }
            int pos = ciag.IndexOf(search, 0, true);
            if (pos == -1) { return ciag; }
            return ciag.Remove(pos, ciag.Length);
        }
    }
}
