using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.International.Converters.PinYinConverter;

namespace ToPinyin
{
    class Program
    {
        static void Main()
        {
            var dictionary = new Dictionary<string, string>();
            var wordList = new List<string>();
            var conson = new List<string>
            {
                "b", "p", "m", "f", "d", "t", "n", "l", "k", "g", "h", "j", "q", "x", "zh", "ch", "sh", "r", "z", "c", "s"
                , "y", "w"
            };

            var pyDictionary = new Dictionary<string, int>();

            wordList.AddRange(File.ReadAllLines("words.txt"));

            var pinyin = "";
            using (var reader = new XmlTextReader("dictionary.xml"))
            {
                var chn = "";
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    switch (reader.LocalName)
                    {
                        case "InputString":
                            pinyin = reader.ReadElementContentAsString();
                            break;
                        case "OutputString":
                            chn = reader.ReadElementContentAsString();
                            break;
                        case "DictionaryEntry" when string.IsNullOrEmpty(chn) || string.IsNullOrEmpty(pinyin):
                            continue;
                        case "DictionaryEntry":
                            dictionary.Add(chn, pinyin);
                            chn = "";
                            pinyin = "";
                            break;
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

                    if (pyChars[0] + pyChars[1].ToString() != o) continue;
                    pinyinStrs[0] = pinyinStrs[0].Replace(o, "");
                    break;
                }

                //if (pinyinStrs[0].Length != 1 && !pinyinStrs[0].Contains("ng")) pinyinStrs[0] = pinyinStrs[0].Replace("i", "");

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
            
            var resultWord = new List<string>();
            foreach (var item in pyDictionary.OrderBy(o => o.Value))
            {
                Console.WriteLine(item.Key + ":" + item.Value);
                resultWord.Add($"{item.Key}:{item.Value}");
                File.AppendAllText($"./tones/{item.Key.Split(' ')[0]}.txt", $"{item.Key}:{item.Value}{Environment.NewLine}");
            }
            File.WriteAllLines("resultWord.txt", resultWord);
            Console.ReadKey();
        }
    }
}
