using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CrawlerConsole
{
    class Database
    {
        private MySqlConnection conn;

        public void openConnection(string server, string port, string usr, string pswd, string db) { 
            //Connect to Database.
            string myConnectionString = @"server=" + server + ";port=" + port + ";userid=" + usr + ";password=" + pswd + ";database=" + db + ";";
            Console.Clear();
            Console.WriteLine("Connecting to MySQL server");
            try {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                // Check is the connection is established equals an open statement.
                if (conn.State.Equals(System.Data.ConnectionState.Open))
                {
                    // Clear the screen and promt the connection establishment.
                    Console.Clear();
                    Console.WriteLine("Connecting to MySQL server: Established");
                }

            }
            catch(MySqlException mysqlEX){
                // Check what error code is given during connection failure.
                switch (mysqlEX.Number)
                {
                    case 0:
                        // Promt if it cannot connect to the database at all.
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;
                    case 1042:
                        // Promt if the hostname/ip address is incorrect.
                        Console.WriteLine("Can't get hostname address. Check your internet connection. If does not solve, contact Administrator");
                        break;
                    case 1045:
                        // Promt if the user and/or password is invalid.
                        Console.WriteLine("Invalid username/password");
                        break;
                }

            }

        }

        public void closeConnection()
        {
            //Close currenct connection to the database.
            conn.Close();
            Console.WriteLine("MySQL connection closed");
        }

        public bool getConnectionStatus() {
            if (conn.State.Equals(System.Data.ConnectionState.Open))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        public bool checkDuplication(string vacId, string siteName, string function)
        {
            /*
             * This part checks for double items.
             * If the record already exist, it returns true.
             */
            MySqlCommand commm = conn.CreateCommand();

            // Create new select query.
            commm.CommandText = "SELECT * FROM vacancy WHERE siteName = ?site AND articleNr = ?vid AND function = ?fun;";
            // Bind variable to the position.
            commm.Parameters.AddWithValue("site", siteName);
            commm.Parameters.AddWithValue("vid", vacId);
            commm.Parameters.AddWithValue("fun", function);
            // New reader and execute query
            MySqlDataReader reader = commm.ExecuteReader();

            //Check if the reader has row, this indicates if there is already a record.
            if (reader.HasRows)
            {
                // Close currenct reader and return True
                reader.Close();
                return true;
            }
            else
            {
                // Close currenct reader and return False
                reader.Close();
                return false;
            }

        }

        public void pushStatus(int crawlerId, int status) {
            MySqlCommand comm = conn.CreateCommand();
            comm.CommandText = "UPDATE crawler SET status = '" + status + "' WHERE CID = '" + crawlerId + "' AND crawlerName = 'vacancyCrawler';";
            comm.ExecuteNonQuery();
        }


        public void pushData(string vacId, string siteName, string function, string education, string region, string employment, string experience, string available, string hours, string salary, string url, string employer, string mainBody)
        {
            //check for double records
            if (checkDuplication(vacId, siteName, function) != true)
            {
                // Create a command instance
                MySqlCommand comm = conn.CreateCommand();

                // New insert query.
                comm.CommandText = "INSERT INTO vacancy(articleNr, function, education, region, employment, experience, available, contractHours, salary, webUrl, siteName, employer, mainBody) VALUES(@vid, @func, @edu, @reg, @emp, @exp, @ava, @hrs, @sal, @url, @wnm, @com, @mby)";
                // Bind Variables to their position.
                comm.Parameters.AddWithValue("@vid", vacId);
                comm.Parameters.AddWithValue("@func", function);
                comm.Parameters.AddWithValue("@edu", education);
                comm.Parameters.AddWithValue("@reg", region);
                comm.Parameters.AddWithValue("@emp", employment);
                comm.Parameters.AddWithValue("@exp", experience);
                comm.Parameters.AddWithValue("@ava", available);
                comm.Parameters.AddWithValue("@hrs", hours);
                comm.Parameters.AddWithValue("@sal", salary);
                comm.Parameters.AddWithValue("@url", url);
                comm.Parameters.AddWithValue("@wnm", siteName);
                comm.Parameters.AddWithValue("@com", employer);
                comm.Parameters.AddWithValue("@mby", mainBody);
                //Execute query
                comm.ExecuteNonQuery();

                Console.WriteLine("Status: Recorded - Record has been added.");

            }
            else { 
                // Message if record already does exist.
                Console.WriteLine("Status: Skipped - Record does already exist."); 
            }
        }
    }
}
