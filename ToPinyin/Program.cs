using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.International.Converters.PinYinConverter;

namespace ToPinyin
{
    class Program
    {
        static void Main()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<string> wordList = new List<string>();

            wordList.AddRange(File.ReadAllLines("words.txt"));

            using (XmlTextReader reader = new XmlTextReader("dictionary.xml"))
            {
                string chn = "";
                string pinyin = "";
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.LocalName == "InputString")
                        pinyin = reader.ReadElementContentAsString();
                    else if (reader.LocalName == "OutputString")
                        chn = reader.ReadElementContentAsString();
                    else if (reader.LocalName == "DictionaryEntry")
                    {
                        if (string.IsNullOrEmpty(chn) || string.IsNullOrEmpty(pinyin)) continue;
                        dictionary.Add(chn, pinyin);
                        chn = "";
                        pinyin = "";
                    }
                }
            }

            foreach (var item in wordList)
            {
                var str = item.Split(':')[0];
                //Console.WriteLine(item);
                if (dictionary.ContainsKey(str)) Console.WriteLine(str + dictionary[str]);
                else
                {
                    var pinyinStrs = str.ToCharArray().ToList()
                        .Select(c => Regex.Replace(new ChineseChar(c).Pinyins[0], @"\d", "").ToLower());
                    Console.WriteLine(str + string.Join(" ", pinyinStrs));
                }
            }

            Console.ReadKey();
        }
    }
}
