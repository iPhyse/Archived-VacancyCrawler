using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerConsole
{
    class JobSelectie : BasicStruct
    {
        public void parseUrlData(Array URLArray)
        {
            WebClient client = new WebClient();
            Database sqlDB = new Database();
            configuration conf = new configuration();
            Status st = new Status();

            //Set the crawler status to online, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(2, 1);

            int count = 0; //Logging data

            //Static search options, if contains*
            string[] educationArray = { "Universitair", "HBO", "MBO", "LBO", "VWO", "HAVO", "VMBO / MAVO" };
            string[] regionArray = { "Nederland", "Drenthe", "Flevoland", "Friesland", "Gelderland", "Groningen", "Limburg", "Noord-Brabant", "Noord-Holland", "Overijssel", "Utrecht", "Zeeland", "Zuid-Holland", "buitenland", "Andorra", "Argentinië", "Australië", "Azië (pacific)", "België", "Bulgarije", "Canada", "Chili", "China", "Colombia", "Denemarken", "Duitsland", "Egypte", "Filipijnen", "Finland", "Frankrijk", "Griekenland", "Hong Kong", "Ierland", "India", "Indonesië", "Italië", "Japan", "Luxemburg", "Maleisië", "Marokko", "Mexico", "Oostenrijk", "Peru", "Polen", "Portugal", "Servië", "Singapore", "Spanje", "Taiwan", "Thailand", "Tsjechië", "Turkije", "Verenigd Koninkrijk", "Verenigde Staten", "Zuid-Afrika", "Zweden", "Zwitserland" };
            string[] employmentArray = { "Vakantiewerk", "Vast", "Tijdelijk", "Freelance", "Afstudeerstage", "ZZP", "Interim", "Bijbaan", "Meeloopstage", "Franchise", "Thuiswerk" };
            string[] experienceArray = { "3 tot 5 jaar", "1 tot 3 jaar", "minder dan 1 jaar", "5 tot 10 jaar", "10 tot 15 jaar" };
            string[] availableArray = { "Binnen 2 maanden", "Binnen 3 maanden", "Binnen een maand", "Direct", "In overleg" };
            string[] hoursArray = { "0 8 uur", "8 16 uur", "16 24 uur", "24 32 uur", "32 40 uur" };

            StreamWriter file = new System.IO.StreamWriter("./jobselectie.txt");

            //Set up a new Connection to the database.
            sqlDB.openConnection(conf.getSQLServerIP(), conf.getSQLServerPort(), conf.getSQlUsername(), conf.getSQLPassword(), conf.getSQLDB());

            foreach (string url in URLArray)
            {
                ++count;
                Stream stream = client.OpenRead(new Uri(url));
                StreamReader reader = new StreamReader(stream);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(reader.ReadToEnd());

                string vacancyNum = "Geen";
                string function = "";
                string education = "";
                string region = "";
                string employment = "";
                string experience = "";
                string available = "";
                string hours = "";
                string salary = "";
                string employer = "";
                string mainBody = "";

                bool refOpt = false;
                int cnt = 0;

                Console.WriteLine("\nRecord: " + count);
                try
                {
                     
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class=\"field\"]"))
                    {
                        foreach (HtmlNode childNode in node.ChildNodes)
                        {
                            string temp = childNode.InnerText;
                            string reptemp = temp.Replace("- ", "");

                            // Kinda tricky, needs to be tested
                            if(refOpt == true){
                                if (cnt == 1)
                                {
                                    vacancyNum = reptemp;
                                    refOpt = false;
                                }
                                else { cnt++; }
                                
                            }
                            if (temp.Contains("Referentienummer"))
                            {
                                refOpt = true;
                            }

                            if(temp.Contains("€")){
                                salary = temp.Remove(0,2);
                            }

                            if (educationArray.Contains(reptemp))
                            {
                                if (education.Length > 1) {
                                    education += ", ";
                                }
                                education += reptemp;
                            }

                            if (employmentArray.Contains(reptemp)) {
                                if (employment.Length > 1)
                                {
                                    employment += ", ";
                                }
                                employment += reptemp;
                            }

                            if(hoursArray.Contains(reptemp)){
                                if (hours.Length > 1)
                                {
                                    hours += ", ";
                                }
                                hours += reptemp;
                            }

                            if (experienceArray.Contains(reptemp))
                            {
                                if (experience.Length > 1)
                                {
                                    experience += ", ";
                                }
                                experience += reptemp;
                            }

                            if (regionArray.Contains(reptemp))
                            {
                                if (region.Length > 1)
                                {
                                    region += ", ";
                                }
                                region += reptemp;
                            }

                        }

                        foreach (HtmlNode node2 in doc.DocumentNode.SelectNodes("//div[@class=\"vacature-body clearfix\"]"))
                        {
                            mainBody = node2.InnerText;
                        }

                        try
                        {
                            employer = doc.DocumentNode.SelectSingleNode("//div[@class=\"field\"]/div/a").InnerText;
                        }
                        catch (Exception e) { }
                        
                    }

                    
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//h1[@id=\"page-title\"]"))
                    {
                        string rawFunction = node.InnerText;

                        function = rawFunction;
                    }

                    if (salary == "")
                    {
                        salary = "Onderhandelbaar";
                    }

                    Console.WriteLine("Vacature/Referentie nr: " + vacancyNum);
                    Console.WriteLine("Functie: " + function);
                    Console.WriteLine("Opleiding: " + education);
                    Console.WriteLine("Regio: " + region);
                    Console.WriteLine("Dienstverband: " + employment);
                    Console.WriteLine("Werk ervaring: " + experience);
                    Console.WriteLine("Contract uren: " + hours);
                    Console.WriteLine("(Start) Salaris: " + salary);
                    Console.WriteLine("Werkgever: " + employer);
                    Console.WriteLine("Web URL: " + url);

                    file.WriteLine("Record [" + count + "]");
                    file.WriteLine("Vacature/Referentie nr:  nr: " + vacancyNum);
                    file.WriteLine("Functie: " + function);
                    file.WriteLine("Opleiding: " + education);
                    file.WriteLine("Regio: " + region);
                    file.WriteLine("Dienstverband: " + employment);
                    file.WriteLine("Werk ervaring: " + experience);
                    file.WriteLine("Contract uren: " + hours);
                    file.WriteLine("(Start) Salaris: " + salary);
                    file.WriteLine("Werkgever: " + employer);
                    file.WriteLine("Web URL: " + url);
                    file.WriteLine(" ");

                    /*
                     * Re-check if connection to the database is still open.
                     * If it's not, reconnect to the database
                     */
                    if (sqlDB.getConnectionStatus() != true)
                    {
                        sqlDB.openConnection(conf.getSQLServerIP(), conf.getSQLServerPort(), conf.getSQlUsername(), conf.getSQLPassword(), conf.getSQLDB());
                    }
                    sqlDB.pushData(vacancyNum, "Jobselectie", function, education, region, employment, experience, available, hours, salary, url, employer, mainBody);
        
                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    ///Set the crawler status to failue, 0 = offline, 1 = online, -1 = failure.
                    st.OnProcessStatus(2, -1);
                }

            }

            file.Close();
            sqlDB.closeConnection();
            //Set the crawler status to offline, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(2, 0);
        }
    }
}
