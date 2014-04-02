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
    class NatVacBank : BasicStruct
    {
        public void parseUrlData(Array URLArray)
        {

            //TODO: getting data
            WebClient client = new WebClient();
            Database sqlDB = new Database();
            configuration conf = new configuration();
            Status st = new Status();

            //Set the crawler status to online, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(1, 1);

            string filterId = "//dl[@class=\"referentie\"]";
            string filtertitle = "//title";
            string filterDetails = "//dl[@class=\"details\"]";
            string filterMainBody = "//div[@id=\"vacature-details\"]";
            int count = 0;

            //Static search options, if contains*
            string[] educationArray = { "Postdoctoraal", "WO", "HBO", "MBO", "VWO", "HAVO", "VMBO/Mavo", "LBO", "Lagere school" };
            string[] employmentArray = { "Vast", "Stage", "Tijdelijk", "Bijbaan", "Interim", "Freelance", "ZZP" };
            string[] experienceArray = { "Student", "Starter", "Ervaren", "Leidinggevend", "Senior management", "Onbekend" };

            StreamWriter file = new System.IO.StreamWriter("./NationaleVacatureBank.txt");

            //Set up a new Connection to the database.
            sqlDB.openConnection(conf.getSQLServerIP(), conf.getSQLServerPort(), conf.getSQlUsername(), conf.getSQLPassword(), conf.getSQLDB());


            foreach (string url in URLArray)
            {

                ++count;
                Stream stream = client.OpenRead(new Uri(url));
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
                string mainBody = "";

                Console.WriteLine("\nRecord: " + count);
                try
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterId))
                    {
                        HtmlNode vacNumber = node.SelectSingleNode("./dd");
                        vacancyNum = vacNumber.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filtertitle))
                    {
                        string rawTitle = node.InnerText;
                        string[] splitTitle = rawTitle.Split('|');
                        function = splitTitle[0];
                        employer = splitTitle[1];
                        region = splitTitle[2].Replace(" ", "");

                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterMainBody))
                    {
                        mainBody += node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterDetails))
                    {
                        foreach (HtmlNode childNode in node.ChildNodes)
                        {
                            string tempText = childNode.InnerText;

                            //Education
                            if (educationArray.Contains(tempText))
                            {
                                if (education.Length > 1)
                                {
                                    education += ", ";
                                }
                                education += tempText;
                            }

                            //Employment
                            if (employmentArray.Contains(tempText))
                            {
                                if (employment.Length > 1)
                                {
                                    employment += ", ";
                                }
                                employment = tempText;
                            }

                            //Experience
                            if (experienceArray.Contains(tempText))
                            {
                                if (experience.Length > 1)
                                {
                                    experience += ", ";
                                }
                                experience += tempText;
                            }

                            //hours
                            if (tempText.Contains("uur per"))
                            {
                                hours += tempText;
                            }


                            //salary 
                            if (tempText.Contains("€"))
                            {
                                salary = tempText;
                            }
                        }
                    }

                    if(experience == ""){
                        experience = "Onbekend";
                    }

                    if (salary == ""){
                        salary = "Onderhandelbaar";
                    }

                    Console.WriteLine("Vacature/Referentie nr:" + vacancyNum);
                    Console.WriteLine("Functie: " + function);
                    Console.WriteLine("Opleiding: " + education);
                    Console.WriteLine("Regio: " + region);
                    Console.WriteLine("Dienstverband: " + employment);
                    Console.WriteLine("Werk ervaring: " + experience);
                    Console.WriteLine("Contract uren: " + hours);
                    Console.WriteLine("(Start) Salaris: " + salary);
                    Console.WriteLine("Bedrijf: " + employer);
                    Console.WriteLine("Web URL: " + url);

                    file.WriteLine("Record [" + count + "]");
                    file.WriteLine("Vacature/Referentie nr: " + vacancyNum);
                    file.WriteLine("Functie: " + function);
                    file.WriteLine("Opleiding: " + education);
                    file.WriteLine("Regio: " + region);
                    file.WriteLine("Dienstverband: " + employment);
                    file.WriteLine("Werk ervaring: " + experience);
                    file.WriteLine("Contract uren: " + hours);
                    file.WriteLine("(Start) Salaris: " + salary);
                    file.WriteLine("Bedrijf: " + employer);
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
                    sqlDB.pushData(vacancyNum, "Nationale Vacaturebank", function, education, region, employment, experience, available, hours, salary, url, employer, mainBody);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //Set the crawler status to failure, 0 = offline, 1 = online, -1 = failure.
                    st.OnProcessStatus(1, -1);
                }

            }
            // Close database connection if the script is finished

            file.Close();
            sqlDB.closeConnection();
            //Set the crawler status to offline, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(1, 0);
        }
        
    }
}
