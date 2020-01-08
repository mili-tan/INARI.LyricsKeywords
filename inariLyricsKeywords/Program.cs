using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JiebaNet.Segmenter;

namespace InariLyricsKeywords
{
    static class Program
    {
        static void Main()
        {
            List<string> texts = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(@".\lyrics");
            foreach (var itemFile in dir.GetFiles("*.txt"))
            {
                texts.AddRange(File.ReadAllLines(itemFile.FullName));
                Console.WriteLine(itemFile.FullName);
            }
            Console.Clear();

            var jieba = new JiebaSegmenter();
            var dict = new Dictionary<string,int>();

            foreach (var item in texts)
            {
                var segsCut = jieba.Cut(Regex.Replace(item, @"[^\u4e00-\u9fa5]+", "").Replace(" ","")).ToArray();
                foreach (var i in segsCut)
                {
                    if (dict.ContainsKey(i)) dict[i] += 1;
                    else dict.Add(i, 1);
                }
                Console.WriteLine(string.Join(" ", segsCut));
            }
            Console.Clear();

            var resultFull = new List<string>();
            foreach (var item in dict.OrderBy(o => o.Value))
            {
                Console.WriteLine($"{item.Key}:{item.Value}");
                resultFull.Add($"{item.Key}:{item.Value}");
            }
            File.WriteAllLines("resultFull.txt", resultFull);
            Console.Clear();

            var resultWord = new List<string>();
            foreach (var item in dict.OrderBy(o => o.Value))
            {
                if (item.Key.Length <= 1 || item.Value <= 2) continue;
                Console.WriteLine($"{item.Key}:{item.Value}");
                resultFull.Add($"{item.Key}:{item.Value}");
            }
            File.WriteAllLines("resultWord.txt", resultWord);
            Console.ReadKey();
        }
    }
}
