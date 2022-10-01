using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen
{
    public static class StringExtensions
    {
        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
            {
                return str;
            }

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
