using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scryfall_Image_Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            String textFile = args[0];
            Console.WriteLine(string.Format(@"Reading {0} for card names", textFile));
            if (!Directory.Exists("./output"))
            {
                Directory.CreateDirectory("./output");
            }

            var lines = File.ReadAllLines(textFile);
            foreach (string line in lines)
            {
                string cardName = line;
                if (line.Length == 0) continue;
                if (char.IsDigit(line[0]))
                {
                    cardName = line.Substring(line.IndexOf(' ')+1);
                }
                Console.WriteLine(String.Format("Getting image for {0}",cardName));
                if (File.Exists(String.Format("./output/{0}.png", cardName)))
                {
                    Console.WriteLine("Found image to already exist locally, skipping");
                    continue;
                }
                String uriCardName = Uri.EscapeUriString(cardName);
                String url = JsonConvert.DeserializeObject<RootObject>(Get(String.Format("https://api.scryfall.com/cards/named?fuzzy={0}", uriCardName))).image_uris.png;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(url), String.Format(@"./output/{0}.png",cardName));
                }
                Thread.Sleep(10);
            }

            Console.WriteLine("Done, images downloaded to: " + Path.Combine(Directory.GetCurrentDirectory(),"output"));
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
