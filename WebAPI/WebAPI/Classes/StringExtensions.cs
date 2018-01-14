using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Classes
{
    public static class StringExtensions
    {
        public static bool CompareCaseIgnorant(this string str, string otherStr)
        {
            return string.Equals(str, otherStr, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}