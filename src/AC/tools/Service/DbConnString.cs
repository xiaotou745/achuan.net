using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AC.Tools.Service
{
    public class DbConnString
    {
        public static IList<string> GetDbServers()
        {
            return (from ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings where connString.Name != "LocalSqlServer" && connString.Name != "OraAspNetConString" select connString.Name).ToList();

            
        }

        public static string GetConnString(string dbServer)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[dbServer];
            if(connectionStringSettings == null)
            {
                return string.Empty;
            }
            return connectionStringSettings.ConnectionString;
        }
    }
}