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
    class Jobbird : BasicStruct
    {
        public void parseUrlData(Array URLArray) {
            WebClient client = new WebClient();
            Database sqlDB = new Database();
            configuration conf = new configuration();
            Status st = new Status();

            //Set the crawler status to online, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(3, 1);

            string filterId = "//input[@id=\"edit-content-id\"]";
            string filtertitle = "//h1[@class=\"vacature-titel-blok\"]";
            string filterItem = "//div[@class=\"field-item\"]";
            string filterMainBody = "//div[@class=\"vacatureSubtitels\"]";
            int count = 0; //Logging data

            //Static search options, if contains*
            string[] educationArray = { "Universitair", "HBO", "MBO", "LBO", "VWO", "HAVO", "VMBO / MAVO" };
            string[] regionArray = { "Nederland", "Drenthe", "Flevoland", "Friesland", "Gelderland", "Groningen", "Limburg", "Noord-Brabant", "Noord-Holland", "Overijssel", "Utrecht", "Zeeland", "Zuid-Holland", "buitenland", "Andorra", "Argentinië", "Australië", "Azië (pacific)", "België", "Bulgarije", "Canada", "Chili", "China", "Colombia", "Denemarken", "Duitsland", "Egypte", "Filipijnen", "Finland", "Frankrijk", "Griekenland", "Hong Kong", "Ierland", "India", "Indonesië", "Italië", "Japan", "Luxemburg", "Maleisië", "Marokko", "Mexico", "Oostenrijk", "Peru", "Polen", "Portugal", "Servië", "Singapore", "Spanje", "Taiwan", "Thailand", "Tsjechië", "Turkije", "Verenigd Koninkrijk", "Verenigde Staten", "Zuid-Afrika", "Zweden", "Zwitserland" };
            string[] employmentArray = { "Vast", "Uitzenden/Tijdelijk", "Stage", "Freelance", "Bijbaan", "ZZP", "Thuiswerk", "Vakantiewerk", "Detachering/Interim", "Leer-werk overeenkomst", "Vrijwilliger", "Zelfstandig/Franchise" };
            string[] experienceArray = { "0 (Starter)", "1-3 jaar", "3-5 jaar", "5-10 jaar", "10 jaar of meer" };
            string[] availableArray = { "Binnen 2 maanden", "Binnen 3 maanden", "Binnen een maand", "Direct", "In overleg" };
            string[] hoursArray = { "0-8 uur", "8-16 uur", "16-24 uur", "24-32 uur", "32-40 uur" };
            string[] driverLicenceArray = { "A - motorfiets", "B - personenauto", "C - vrachtwagen", "D - bus" };
            string[] salaryArray = { "€ 0 - € 1750", "€ 1750 - € 2500", "€ 2500 - € 3500", "€ 3500 - € 5000", "€ 5000 - en meer" };

            StreamWriter file = new System.IO.StreamWriter("./jobbird.txt");

            //Set up a new Connection to the database.
            sqlDB.openConnection(conf.getSQLServerIP(), conf.getSQLServerPort(), conf.getSQlUsername(), conf.getSQLPassword(), conf.getSQLDB());

            foreach(string url in URLArray){

                ++count;
                string fixedURL = "http://www.jobbird.com" + url;
                Stream stream = client.OpenRead(new Uri(fixedURL));
                StreamReader reader = new StreamReader(stream);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(reader.ReadToEnd());

                string vacancyNum = "";
                string function = "";
                string education = "";
                string region = "";
                string employment = "";
                string experience = "";
                string available = "";
                string hours = "";
                string salary = "";
                string employer = "";
                string body = "";
                string mainBody = "";

                Console.WriteLine("\nRecord: " + count);
                try
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@id=\"content-contact\"]"))
                    {
                        employer = node.FirstChild.InnerText;
                    }

                    foreach( HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class=\"section-vacature\"]"))
                    {
                        body += node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterId))
                    {
                        HtmlAttribute att = node.Attributes["value"];
                        vacancyNum = att.Value;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filtertitle))
                    {
                        function = node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterMainBody))
                    {
                        mainBody = node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterItem))
                    {
                        string att = node.InnerText;

                        if(educationArray.Contains(att)) {
                            if(education.Length > 1){
                                education += ", ";
                            }
                            education += att;
                        }

                        if (regionArray.Contains(att)) {
                            if (region.Length > 1)
                            {
                                region += ", ";
                            }
                            region += att;
                        }

                        if(employmentArray.Contains(att)){
                            if (employment.Length > 1)
                            {
                                employment += ", ";
                            }
                            employment = att;
                        }

                        if (experienceArray.Contains(att))
                        {
                            if (experience.Length > 1)
                            {
                                experience += ", ";
                            }
                            experience += att;
                        }

                        if (hoursArray.Contains(att))
                        {
                            if (hours.Length > 1)
                            {
                                hours += ", ";
                            }
                            hours += att;
                        }

                        if (salaryArray.Contains(att))
                        {
                            if (salary.Length > 1)
                            {
                                salary += ", ";
                            }
                            salary += att;
                        }
                    }

                    Console.WriteLine("Vacature/Referentie nr:  nr: " + vacancyNum);
                    Console.WriteLine("Functie: " + function);
                    Console.WriteLine("Opleiding: " + education);
                    Console.WriteLine("Regio: " + region);
                    Console.WriteLine("Dienstverband: " + employment);
                    Console.WriteLine("Werk ervaring: " + experience);
                    Console.WriteLine("Contract uren: " + hours);
                    Console.WriteLine("(Start) Salaris: " + salary);
                    Console.WriteLine("Werkgever: " + employer);
                    Console.WriteLine("Web URL: " + fixedURL);

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
                    file.WriteLine("Web URL: " + fixedURL);
                    file.WriteLine(" ");

                    /*
                     * Re-check if connection to the database is still open.
                     * If it's not, reconnect to the database
                     */
                    if (sqlDB.getConnectionStatus() != true) {
                        sqlDB.openConnection(conf.getSQLServerIP(), conf.getSQLServerPort(), conf.getSQlUsername(), conf.getSQLPassword(), conf.getSQLDB());
                    }
                    sqlDB.pushData(vacancyNum, "Jobbird", function, education, region, employment, experience, available, hours, salary, "http://www.jobbird.nl" + url, employer, mainBody);

                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    //Set the crawler status to failure, 0 = offline, 1 = online, -1 = failure.
                    st.OnProcessStatus(3, -1);
                }

            }

            file.Close();
            sqlDB.closeConnection();
            //Set the crawler status to offline, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(3, 0);
        }
    }
}
