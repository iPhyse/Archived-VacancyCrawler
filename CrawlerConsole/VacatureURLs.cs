using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerConsole
{
    class VacatureURLs
    {
        public Array getVacURLs(int pCount, string URL, string filter, bool log, string[] session)
        {
            //List<string> urlsListSession = new List<string>();
            List<string> urlsList = new List<string>();
            WebClient client = new WebClient();
            Status st = new Status();
            int i = 0;
            int count = 0;

            while (i < pCount)
            {
                Stream stream = client.OpenRead(new Uri("http://" + URL + i));
                StreamReader reader = new StreamReader(stream);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(reader.ReadToEnd());

                try
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filter))
                    {
                        HtmlAttribute att = node.Attributes["href"];
                        if (!session.Contains(att.Value))
                        {
                            ++count;
                            if (log == true)
                            {
                                Console.WriteLine("[" + count + "]  " + att.Value);
                            }

                            urlsList.Add(att.Value);

                        }
                        
                    }
                }
                catch (Exception) { 
                    //Nothing 
                }

                i++;

            }

            Console.WriteLine("Fetching url links done...\n");
            //List to array
            Array urlArray = urlsList.ToArray();
            return urlArray;
        }
    }
}
