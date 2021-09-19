using System;
using System.IO;
using System.Collections;
using HtmlAgilityPack;
using System.Linq;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Collections.Generic;

namespace epub2dict
{
    class Program
    {
        static void Main(string[] args)
        {
            var epubFolder = ".\\source";
            var filenames = Directory.GetFiles(epubFolder);
            var entries = new List<Entry>();

            foreach (var filename in filenames)
            {
                Console.Write($"Extracting from '{filename}' ");
                string filetext = File.ReadAllText(filename);

                var doc = new HtmlDocument();
                doc.LoadHtml(filetext);
                var extractedEntries = EpubParser.ExtractEntries(doc);
                entries.AddRange(extractedEntries);
                Console.WriteLine($"found {extractedEntries.Count()} entries");
            }

            foreach (var entry in entries)
            {
                entry.Key = entry.Key.FixKey();
                entry.Index = entry.Key.GenerateIndexedChars();
            }
                        
            //https://kindlebilgideposu.wordpress.com/howtocreatekobodictionaries/
            CreateIndexTxt(entries);
            
            var entriesByIndex = from r in entries
                group r by r.Index into g
                select new {
                        Index = g.Key,
                        Entries = g.Select(e => e).ToList()
                    };
            
            foreach (var file in entriesByIndex)
            {
                CreateDictionaryHtml(file.Entries);
            }
        }

        private static void CreateIndexTxt(List<Entry> entries)
        {
            File.Delete(".\\output\\index.txt");

            string[] lines = entries.Select(e => e.Key).ToArray();

            using (StreamWriter outputFile = new StreamWriter(".\\output\\index.txt"))
            {
                outputFile.NewLine = "\n";
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        private static void CreateDictionaryHtml(List<Entry> entries)
        {
                var filename = ".\\output\\" + entries[0].Index + ".html";
                
                using (StreamWriter outputFile = new StreamWriter(filename, append: false))
                {
                    outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><html>");

                    foreach (var entry in entries)
                    {
                        outputFile.NewLine = "\n";
                        outputFile.WriteLine($"<w><p><a name=\"{entry.Key}\"/>{entry.Value}</p></w>");
                    }
                    outputFile.WriteLine("</html>");
                }
        }
    }
}
