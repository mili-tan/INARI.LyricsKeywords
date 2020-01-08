using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetLyrics
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = File.ReadAllLines("playlist.csv");
            foreach (var item in list)
            {
                if (File.Exists($"./lyrics/{item}.txt")) continue;
                var jsonStr = new WebClient { Encoding = Encoding.UTF8 }.DownloadString("https://v1.hitokoto.cn/nm/lyric/" + item);
                var json = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                try
                {
                    var text = Regex.Replace(json.lrc.lyric.ToString(), "\\[.*\\]", "");
                    Console.WriteLine(text);
                    File.WriteAllText($"./lyrics/{item}.txt", text);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    try
                    {
                        File.WriteAllText($"./lyrics/{item}.txt", json.lrc.lyric.ToString());
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);

                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
