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
            List<string> conson = new List<string>
            {
                "b", "p", "m", "f", "d", "t", "n", "l", "k", "ɡ", "h", "j", "q", "x", "zh", "ch", "sh", "r", "z", "c", "s"
                , "y", "w"
            };

            Dictionary<string,int> pyDictionary = new Dictionary<string, int>();

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
                List<string> pinyinStrs;
                if (dictionary.ContainsKey(str))
                    pinyinStrs = dictionary[str].Split(' ').ToList();
                else
                    pinyinStrs = str.ToCharArray().ToList()
                        .Select(c => Regex.Replace(new ChineseChar(c).Pinyins[0], @"\d", "").ToLower()).ToList();

                foreach (var o in conson)
                {
                    var pyChars = pinyinStrs[0].ToCharArray();
                    if (pyChars.Length <= 1) continue;
                    if (pyChars[0].ToString() == o)
                    {
                        pinyinStrs[0] = pinyinStrs[0].Replace(o, "");
                        break;
                    }
                    if (pyChars[0] + pyChars[1].ToString() == o)
                    {
                        pinyinStrs[0] = pinyinStrs[0].Replace(o, "");
                        break;
                    }
                }

                //if (pinyinStrs[1].Length > 1)
                //    if (pinyinStrs[1].ToCharArray()[1] == 'h')
                //        pinyinStrs[1] = pinyinStrs[1].Trim().Substring(0, 2);
                //    else
                //        pinyinStrs[1] = pinyinStrs[1].Trim().Substring(0, 1);

                var pyStr = string.Join(" ", pinyinStrs);
                if (pyDictionary.ContainsKey(pyStr))
                    pyDictionary[pyStr] += Convert.ToInt32(item.Split(':')[1]);
                else
                    pyDictionary.Add(pyStr, Convert.ToInt32(item.Split(':')[1]));
            }

            foreach (var item in pyDictionary.OrderBy(o => o.Value))
            {
                Console.WriteLine(item.Key + ":" + item.Value);
            }

            Console.ReadKey();
        }
    }
}
