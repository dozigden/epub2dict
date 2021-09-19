using System;
using System.Collections.Generic;
using System.Linq;
using epub2dict;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace epub2dict
{
    public static class EpubParser
    {
        public static List<Entry> ExtractEntries(HtmlDocument doc)
        {
            var elements = doc.DocumentNode.QuerySelectorAll("div.chapter > *").Where(e => e.GetClasses().Any());
            var res = new List<Entry>();
            foreach (var element in elements)
            {
                if (element.HasClass("chapternoindent"))
                {
                    var possibleKey = element.QuerySelector("strong>span");
                
                    if (possibleKey != null)
                    {
                        var newEntry = new Entry();
                        newEntry.Key = FixKey(possibleKey);
                        newEntry.Value = element.InnerHtml;
                        newEntry.Index = GetFileName(newEntry.Key);
                        res.Add(newEntry);
                    }
                }
                else if (element.HasClass("informalfigure") == false) // This class appears on images, which we cant handle
                {
                    if (res.Any())
                    res.Last().Value += element.InnerHtml;
                } 
            }
            return res;
        }

        private static string GetFileName(string key)
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

        private static string FixKey(HtmlNode possibleKey)
        {
            var key = possibleKey.InnerText;
            //remove trailing .
            if (key.Last() == '.')
            {
                key = key.Substring(0, key.Length - 1);
            }

            //Entries with single commas in
            if (key.Where(c => c == ',').Count() == 1)
            {
                var parts = key.Split(',');
                // Strip 'The' at the start of entries
                if (parts[1].Trim().Equals("The", StringComparison.InvariantCultureIgnoreCase))
                {
                    return parts[0].Trim();
                }
                // one comma is probably a backwards name
                key = parts[1].Trim() + " " + parts[0].Trim();
            }

            return key;
        }
    }
}
