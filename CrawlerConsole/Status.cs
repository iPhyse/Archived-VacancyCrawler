using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerConsole
{
    class Status
    {
        public void OnProcessStatus(int crawlerId, int status)
        {
            Database SQLDB = new Database();
            configuration conf = new configuration();
            SQLDB.openConnection(conf.getSQLServerIP(), conf.getSQLServerPort(), conf.getSQlUsername(), conf.getSQLPassword(), conf.getSQLDB());
            SQLDB.pushStatus(crawlerId, status);
            SQLDB.closeConnection();
        }
        
    }
}
