using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace CrawlerConsole
{
    class Program
    {
        static void Main()
        {
            Status st = new Status();
            Console.Clear();
            int n;
            //int count = 0; //just for testing D=!
            int pageCount = 0;
            string webURL = "";
            string inputExtend = "";
            string nodeSelectFilter = "";
            string inputURL = "";

            // Fixed variables for automated crawling (NatVacbank,Jobselectie, Jobbird & Itjobboard 
            string FIXED_NationaleVac = "";
            string FIXED_Jobselectie = "";
            string FIXED_Jobbird = "";
            string FIXED_ItJobboard = "";
            string FIXED_NationaleVac_filt = "";
            string FIXED_Jobselectie_filt = "";
            string FIXED_Jobbird_filt = "";
            string FIXED_ItJobboard_filt = "";
            int FIXED_NationaleVac_pgs = 0;
            int FIXED_Jobselectie_pgs = 0;
            int FIXED_Jobbird_pgs = 0;
            int FIXED_ItJobboard_pgs = 0;

            Array urls;
            Boolean logging = false;

            //Console setup/layout setting: title, color, etc.
            Console.Title = "Vacature Crawler";
            Console.WindowWidth = 128;
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.OutputEncoding = System.Text.Encoding.Default;

            /*
             * F option for full log in the console.
             * the logging variable can be given to a function.
            */
            Console.WriteLine("FUll log: \"F\", Minimum log: \"M\"\n");
            string fulLog = Console.ReadLine();
            if (fulLog == "F" || fulLog == "f") { logging = true; }
            else { logging = false; }

            // Clear screen of previous options and add the title banner.
            Console.Clear();
            addTitle();

            // Prompt for options.
            // Just for indication.
            Console.WriteLine("Please choose one of the following options:\n");
            Console.WriteLine("[0] Nationale Vacaturebank");
            Console.WriteLine("[1] Jobselectie");
            Console.WriteLine("[2] Jobbird");
            Console.WriteLine("[3] ItJobBoard");
            Console.WriteLine("[9] Crawl all (FIXED variables!)");
            Console.WriteLine("[Q] close the application\n");

            // Check for input (0-9 or Q).
            string inputChoice = Console.ReadLine();

            // Stop the application if option Q is given.
            if (inputChoice == "q" || inputChoice == "Q")
            {
                // Destroy session and exit application.
                Environment.Exit(0);
            }

            /*
             * Try to parse to int and create a boolean: true/false.
             * isNumeric = True/False, n = interger.
            */
            bool isNumeric = int.TryParse(inputChoice, out n);

            //Clear the screen, just for the overview.
            Console.Clear();

            /*
             * Check if the input above could be parsed to interger,
             * else reset the application.
            */
            if (isNumeric)
            {
                //Suffix is used to add an index for the page which we are filtering.
                string suffix = null;
                switch (n)
                {
                    case 0:
                        /*
                         * Case 0 is Nationale vacaturebank.
                         * This site has no suffix index.
                        */
                        Console.WriteLine("Nationale Vacaturebank selected.\n");
                        webURL = "www.nationalevacaturebank.nl/vacature/zoeken/";
                        nodeSelectFilter = "//a[@class=\"span-18 result-item-link\"]";
                        //Suffix is not needed.
                        break;
                    case 1:
                        /*
                         * Case 1 is Jobselectie.
                         * This site has a "&page" index suffix.
                        */
                        Console.WriteLine("Jobselectie.nl selected.\n");
                        webURL = "www.jobselectie.nl/vacatures/?";
                        nodeSelectFilter = "//span[@class=\"field-content\"]/a";
                        // suffix is added to the end of the url string.
                        suffix = "&page=";
                        break;
                    case 2:
                        /*
                         * Case 2 is Jobbird.
                         * This site has a "&page" index suffix.
                        */
                        Console.WriteLine("Jobbird selected.\n");
                        webURL = "jobbird.com/nl/kandidaat/vacature-zoekresultaat/?";
                        nodeSelectFilter = "//dl[@class=\"search-results apachesolr_search-results\"]/a";
                        // suffix is added to the end of the url string.
                        suffix = "&page=";
                        break;
                    case 3:
                        /*
                         * Case 3 is Itjobboard.
                         * This site has a "&page" index suffix.
                        */
                        Console.WriteLine("Itjobboard selected.\n");
                        webURL = "www.itjobboard.nl/vacatures/";
                        nodeSelectFilter = "//a[@itemprop=\"url\"]";
                        // suffix is added to the end of the url string.
                        suffix = "&Page=";
                        break;
                    case 9:
                        Console.WriteLine("Crawl all (FIXED variables!) selected.\n");
                        //Fixed variables for Nationale vacaturebank
                        FIXED_NationaleVac = "www.nationalevacaturebank.nl/vacature/zoeken/overzicht/wijzigingsdatum/query//distance/30/output/html/industries/ICT/items_per_page/50/page/";
                        FIXED_NationaleVac_filt = "//a[@class=\"span-18 result-item-link\"]";
                        FIXED_NationaleVac_pgs = 26; // 26 | Lower for testing
                        //Fixed variables for Jobselectie
                        FIXED_Jobselectie = "www.jobselectie.nl/vacatures?f[0]=field_functiegroep%253Aname%3AAutomatisering/Internet/ICT&page=";
                        FIXED_Jobselectie_filt = "//span[@class=\"field-content\"]/a";
                        FIXED_Jobselectie_pgs = 160; // 160 | lower for testing
                        //Fixed variables for Jobbird
                        FIXED_Jobbird = "www.jobbird.com/nl/kandidaat/vacature-zoekresultaat?filters=tid_75-tid_136-tid_1245&rows=50&page=";
                        FIXED_Jobbird_filt = "//dl[@class=\"search-results apachesolr_search-results\"]/a";
                        FIXED_Jobbird_pgs = 51; // 51 | Lower for testing
                        //Fixed variables for itJobboard
                        FIXED_ItJobboard = "www.itjobboard.nl/vacatures/alle-banen/alle-locaties/alle/0/relevantie/nl/?source=Search&Currency=EUR&ResultSize=90&Page=";
                        FIXED_ItJobboard_filt = "//a[@itemprop=\"url\"]";
                        FIXED_ItJobboard_pgs = 17; // 17 | Lower for testing
                        break;
                    default:
                        //If one of the upper options isn't selected
                        Console.WriteLine("Option is not known!");
                        Console.ReadKey();

                        //Clear and reset application
                        Console.Clear();
                        Main();
                        break;
                }

                if (n != 9)
                {
                    //Prompt for query
                    Console.WriteLine("Please enter the extention query (Without page number, read manual.txt):\n");
                    inputExtend = Console.ReadLine();

                    //Amount of pages
                    Console.WriteLine("How many pages? (E.g. 10):");

                    pageCount = int.Parse(Console.ReadLine());

                    //Build pre-final link
                    inputURL = webURL + inputExtend + suffix;
                }

            }
            else
            {
                Console.WriteLine("Input is not a number!");
                Console.ReadKey();

                //Clear and reset application
                Console.Clear();
                Main();
            }

            //Clear the screen, just for the overview D=?
            Console.Clear();
            Console.WriteLine("Reading url strings: \n");

            //Start time course
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                VacatureURLs vacatueUrls = new VacatureURLs();
                switch (n)
                {
                    case 0:
                        /*
                         * Case 0 is Nationale vacaturebank
                         * Getting link data per indexed page
                        */
                        urls = vacatueUrls.getVacURLs(pageCount, inputURL, nodeSelectFilter, logging, null);
                        Console.WriteLine(urls.Length + " Results");
                        // New instance of NatVactBank and Parse page data 
                        NatVacBank nvb = new NatVacBank();
                        nvb.parseUrlData(urls);
                        break;
                    case 1:
                        /*
                         * Case 1 is Jobselectie
                         * Getting link data per indexed page
                        */
                        urls = vacatueUrls.getVacURLs(pageCount, inputURL, nodeSelectFilter, logging, null);
                        Console.WriteLine(urls.Length + " Results");
                        // New instance of Jobselectie and Parse page data 
                        JobSelectie js = new JobSelectie();
                        js.parseUrlData(urls);
                        break;
                    case 2:
                        /*
                         * Case 1 is Jobbird
                         * Getting link data per indexed page 
                        */
                        urls = vacatueUrls.getVacURLs(pageCount, inputURL, nodeSelectFilter, logging, null);
                        Console.WriteLine(urls.Length + " Results");
                        // New instance of Jobbird and Parse page data 
                        Jobbird jb = new Jobbird();
                        jb.parseUrlData(urls);
                        break;
                    case 3:
                        /*
                         * Case 1 is Itjobboard
                         * Getting link data per indexed page
                        */
                        urls = vacatueUrls.getVacURLs(pageCount, inputURL, nodeSelectFilter, logging, null);
                        Console.WriteLine(urls.Length + " Results");
                        ItJobBoard itjb = new ItJobBoard();
                        itjb.parseUrlData(urls);
                        break;
                    case 9:

                        //Option for fixed variables
                        string empty = "";
                        Array natVacBankUrls = empty.ToArray();
                        Array JobselectieUrls = empty.ToArray();
                        Array JobbirdUrls = empty.ToArray();
                        Array ItjobboardUrls = empty.ToArray();

                        Array oldUrlSession = empty.ToArray();

                        do {
                            while (!Console.KeyAvailable)
                            {
                                try
                                {
                                    Console.Clear();
                                    Console.WriteLine("Reading url strings: Nationale Vacaturebank: \n");
                                    

                                    string[] foo = oldUrlSession.OfType<object>().Select(o => o.ToString()).ToArray();


                                    //url
                                    natVacBankUrls = vacatueUrls.getVacURLs(FIXED_NationaleVac_pgs, FIXED_NationaleVac, FIXED_NationaleVac_filt, logging, foo);

                                    if (oldUrlSession.Length == 0)
                                    {
                                        oldUrlSession = natVacBankUrls;

                                    }
                                    else if (natVacBankUrls.Length > 0)
                                    {

                                        List<string> newList = natVacBankUrls.OfType<string>().ToList();
                                        List<string> oldList = oldUrlSession.OfType<string>().ToList();

                                        oldList.AddRange(newList);

                                        oldUrlSession = oldList.ToArray();
                                    }

                                    Console.WriteLine(natVacBankUrls.Length + " Results for Nationale Vacaturebank.\n");
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch
                                {
                                    Console.WriteLine("Nationale Vacaturebank is unreachable");
                                    System.Threading.Thread.Sleep(1000);
                                }

                                try
                                {
                                    Console.Clear();
                                    Console.WriteLine("Reading url strings: Jobselectie: \n");
                                    

                                    string[] foo = oldUrlSession.OfType<object>().Select(o => o.ToString()).ToArray();


                                    //url
                                    JobselectieUrls = vacatueUrls.getVacURLs(FIXED_Jobselectie_pgs, FIXED_Jobselectie, FIXED_Jobselectie_filt, logging, foo);
                                    
                                    if (JobselectieUrls.Length > 0)
                                    {

                                        List<string> newList = JobselectieUrls.OfType<string>().ToList();
                                        List<string> oldList = oldUrlSession.OfType<string>().ToList();

                                        oldList.AddRange(newList);

                                        oldUrlSession = oldList.ToArray();
                                    }
                                    Console.WriteLine(JobselectieUrls.Length + " Results for Jobselectie.\n");
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch
                                {
                                    Console.WriteLine("Jobselectie is unreachable");
                                    System.Threading.Thread.Sleep(1000);
                                }

                                try
                                {
                                    Console.Clear();
                                    Console.WriteLine("Reading url strings: Jobbird: \n");
                                    

                                    string[] foo = oldUrlSession.OfType<object>().Select(o => o.ToString()).ToArray();


                                    //url
                                    JobbirdUrls = vacatueUrls.getVacURLs(FIXED_Jobbird_pgs, FIXED_Jobbird, FIXED_Jobbird_filt, logging, foo);
                                    
                                    if (JobbirdUrls.Length > 0)
                                    {

                                        List<string> newList = JobbirdUrls.OfType<string>().ToList();
                                        List<string> oldList = oldUrlSession.OfType<string>().ToList();

                                        oldList.AddRange(newList);

                                        oldUrlSession = oldList.ToArray();
                                    }

                                    Console.WriteLine(JobbirdUrls.Length + " Results for Jobbird.\n");
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch
                                {
                                    Console.WriteLine("Jobbird is unreachable");
                                    System.Threading.Thread.Sleep(1000);
                                }

                                try
                                {
                                    Console.Clear();
                                    Console.WriteLine("Reading url strings: Itjobboard: \n");
                                    

                                    string[] foo = oldUrlSession.OfType<object>().Select(o => o.ToString()).ToArray();


                                    //url
                                    ItjobboardUrls = vacatueUrls.getVacURLs(FIXED_ItJobboard_pgs, FIXED_ItJobboard, FIXED_ItJobboard_filt, logging, foo);

                                    if (ItjobboardUrls.Length > 0)
                                    {

                                        List<string> newList = ItjobboardUrls.OfType<string>().ToList();
                                        List<string> oldList = oldUrlSession.OfType<string>().ToList();

                                        oldList.AddRange(newList);

                                        oldUrlSession = oldList.ToArray();
                                    }

                                    Console.WriteLine(ItjobboardUrls.Length + " Results for Itjobboard\n");
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch
                                {
                                    Console.WriteLine("ItJobboard is unreachable");
                                    System.Threading.Thread.Sleep(1000);
                                }

                                if (natVacBankUrls.Length != 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Parsing data from Nationale Vacaturebank: \n");
                                    NatVacBank nvb_fixed = new NatVacBank();
                                    nvb_fixed.parseUrlData(natVacBankUrls);
                                }

                                if (JobselectieUrls.Length != 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Parsing data from Jobselectie: \n");
                                    JobSelectie js_fixed = new JobSelectie();
                                    js_fixed.parseUrlData(JobselectieUrls);
                                }

                                if (JobbirdUrls.Length != 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Parsing data from Jobbird: \n");
                                    Jobbird jb_fixed = new Jobbird();
                                    jb_fixed.parseUrlData(JobbirdUrls);
                                }

                                if (ItjobboardUrls.Length != 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Parsing data from Itjobboard: \n");
                                    ItJobBoard itjb_fixed = new ItJobBoard();
                                    itjb_fixed.parseUrlData(ItjobboardUrls);
                                }
                            }
                        } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                        
                        break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                //Clear and reset application
                Console.Clear();
                Main();
            }

            //..
            Console.WriteLine("\nDone..");

            //Stop time course and massure the elabsed time.
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format the elabsed time bij hours, minutes, seconds and milliseconds.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Time elapsed: " + elapsedTime);

            Console.ReadKey();
            //Clear and reset application
            Console.Clear();
            Main();
        }


        // Banner for the application, can be called by addTitle().
        public static void addTitle()
        {
            Console.WriteLine(" _   _                 _                  _____                    _");
            Console.WriteLine("| | | |               | |                /  __ \\                  | |");
            Console.WriteLine("| | | | __ _  ___ __ _| |_ _   _ _ __ ___| /  \\/_ __ __ ___      _| | ___ _ __");
            Console.WriteLine("| | | |/ _` |/ __/ _` | __| | | | '__/ _ \\ |   | '__/ _` \\ \\ /\\ / / |/ _ \\ '__|");
            Console.WriteLine("\\ \\_/ / (_| | (_| (_| | |_| |_| | | |  __/ \\__/\\ | | (_| |\\ V  V /| |  __/ |");
            Console.WriteLine(" \\___/ \\__,_|\\___\\__,_|\\__|\\__,_|_|  \\___|\\____/_|  \\__,_| \\_/\\_/ |_|\\___|_|");
            Console.WriteLine("\n\n");
        }
    }

}
