using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Extends
{
    public static class DateTimeExtensions
    {
        public static bool IsSend(this DateTime dt)
        {
            if (DateTime.Compare(dt, DateTime.Now.AddYears(-1)) < 0)
                return false;
            return true;
        }

        public static string GetDateTimeFormated(this DateTime dt)
        {
            return String.Format("{0:dd-MM-yyyy HH:mm}", dt);
        }
    }
}
