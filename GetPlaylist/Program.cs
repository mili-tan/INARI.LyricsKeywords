using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace GetPlaylist
{
    static class Program
    {
        static void Main()
        {
            uint pid = 60079540;
            List<string> list = new List<string>();
            var jsonStr = new WebClient {Encoding = Encoding.UTF8}.DownloadString("https://v1.hitokoto.cn/nm/playlist/" + pid);
            var json = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            foreach (var itemTrack in json.playlist.tracks)
            {
                list.Add(itemTrack.id.ToString());
                Console.WriteLine(itemTrack.id.ToString());
            }

            File.WriteAllLines(pid + "playlist.txt", list);
            Console.ReadKey();
        }
    }
}
