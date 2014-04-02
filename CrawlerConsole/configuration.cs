using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerConsole
{
    class configuration
    {
        string sqlServer =      "127.0.0.1";    //Default: 127.0.0.1
        string sqlServerPort =  "3306";         //Default: 3306
        string sqlUsername =    "root";         //Default: root
        string sqlPassword =    "";             //Default: EMPTY
        string sqlDatabase =    "jobmatcher";   //Default: jobmatcher

        public string getSQLServerIP()
        {
            return sqlServer;
        }

        public string getSQLServerPort()
        {
            return sqlServerPort;
        }

        public string getSQlUsername() 
        {
            return sqlUsername;
        }

        public string getSQLPassword()
        {
            return sqlPassword;
        }

        public string getSQLDB()
        {
            return sqlDatabase;
        }
    }
}
