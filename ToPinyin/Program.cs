using System.Collections.Generic;
using System.Xml;

namespace ToPinyin
{
    class Program
    {
        static void Main()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

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


        }
    }
}
