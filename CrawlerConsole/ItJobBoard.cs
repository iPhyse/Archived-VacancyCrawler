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
    class ItJobBoard : BasicStruct
    {
        public void parseUrlData(Array URLArray)
        {
            WebClient client = new WebClient();
            Database sqlDB = new Database();
            configuration conf = new configuration();
            Status st = new Status();

            //Set the crawler status to online, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(4, 1);

            string filterId = "//input[@id=\"CurrentAdvertId\"]";
            string filterTitle = "//h1[@itemprop=\"title\"]";
            string filterSalary = "//td[@itemprop=\"baseSalary\"]";
            string filterRegion = "//span[@itemprop=\"addressRegion\"]";
            string filterEmployment = "//td[@itemprop=\"employmentType\"]";
            string filterEmployer = "//td[@itemprop=\"name\"]";
            string filterDescription = "//div[@itemprop=\"description\"]";
            string filterMainBody = "//div[@id=\"jobText\"]";
            int count = 0;

            string[] educationArray = { "Postdoctoraal", "WO", "VW", "HBO", "MBO", "VWO", "HAVO", "VMBO/Mavo", "LBO", "Lagere school" };
            string[] experienceArray = { "Student", "Starter", "Ervaren", "Leidinggevend", "Senior management", "Onbekend" };

            StreamWriter file = new System.IO.StreamWriter("./ItJobBoard.txt");

            //Set up a new Connection to the database
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
                string employer = "";
                string experience = "";
                string salary = "";

                string available = "";
                string hours = "";

                string mainBody = "";

                Console.WriteLine("\nRecord: " + count);
                try
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterId))
                    {
                        HtmlAttribute att = node.Attributes["value"];
                        vacancyNum = att.Value;
                        
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterTitle))
                    {
                        function = node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterSalary))
                    {
                        salary = node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterRegion))
                    {
                        region = node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterEmployment))
                    {
                        employment = node.InnerText;
                    }
                    
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterEmployer))
                    {
                        employer = node.InnerText;
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterDescription))
                    {
                        foreach (HtmlNode childNode in node.ChildNodes)
                        {
                            string tempText = childNode.InnerText;

                            //Education
                            foreach (string x in educationArray)
                            {
                                if (!education.Contains(x))
                                {
                                    int i = tempText.IndexOf(x);

                                    if (i != -1)
                                    {
                                        if (education.Length > 1)
                                        {
                                            education += ", ";
                                        }
                                        education += tempText.Substring(i, x.Length);
                                    }
                                }
                            }
                        }
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes(filterMainBody))
                    {
                        mainBody = node.InnerText;
                    }

                    if (education == ""){
                        education = "Onbekend";
                    }

                    if(experience == ""){
                        experience = "Onbekend";
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
                    Console.WriteLine("(Start) Salaris: " + salary);
                    Console.WriteLine("Werkgever: " + employer);
                    Console.WriteLine("Web URL: " + url);

                    file.WriteLine("Record [" + count + "]");
                    file.WriteLine("Vacature/Referentie nr: " + vacancyNum);
                    file.WriteLine("Functie: " + function);
                    file.WriteLine("Opleiding: " + education);
                    file.WriteLine("Regio: " + region);
                    file.WriteLine("Dienstverband: " + employment);
                    file.WriteLine("Werk ervaring: " + experience);
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
                    sqlDB.pushData(vacancyNum, "ItJobboard", function, education, region, employment, experience, available, hours, salary, url, employer, mainBody);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //Set the crawler status to failure, 0 = offline, 1 = online, -1 = failure.
                    st.OnProcessStatus(4, -1);
                }

            }
            file.Close();
            sqlDB.closeConnection();
            //Set the crawler status to offline, 0 = offline, 1 = online, -1 = failure.
            st.OnProcessStatus(4, 0);
        }
    }
}
