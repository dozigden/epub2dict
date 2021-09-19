using System;
using System.Collections.Generic;
using System.Linq;
using epub2dict;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace epub2dict
{
    public static class EntryExtensions
    {
        public static String FixKey(this String key)
        {
            var newKey = key;
            //remove trailing .
            if (newKey.Last() == '.')
            {
                newKey = newKey.Substring(0, newKey.Length - 1);
            }

            //Entries with single commas in
            if (newKey.Where(c => c == ',').Count() == 1)
            {
                var parts = newKey.Split(',');
                // Strip 'The' at the start of entries
                if (parts[1].Trim().Equals("The", StringComparison.InvariantCultureIgnoreCase))
                {
                    return parts[0].Trim();
                }
                // one comma is probably a backwards name
                newKey = parts[1].Trim() + " " + parts[0].Trim();
            }

            return newKey;
        }

        public static string GenerateIndexedChars(this string key)
        {
            var lowerKey = key.ToLowerInvariant();
            var ret ="";
            if (Char.IsLetter(lowerKey[0]))
            {
                ret += lowerKey[0];
            }
            else
            {
               return "11";
            }

            if (lowerKey.Length < 2)
            {
                return ret + "a";
            }

            if (Char.IsLetter(lowerKey[1]))
            {
                ret += lowerKey[1];
            }
            else
            {
               return "11";
            }

            return ret;
        }
    }
}
