using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities
{
    class DatabaseProvider
    {
        static MySqlConnectionStringBuilder ConnectionString;

        public static void Initialize()
        {
            ConnectionString = new MySqlConnectionStringBuilder();
            ConnectionString.Server = Utilities.Config.GetStringElement("WORLD_DATABASE_SERVER");
            ConnectionString.UserID = Utilities.Config.GetStringElement("WORLD_DATABASE_USER");
            ConnectionString.Password = Utilities.Config.GetStringElement("WORLD_DATABASE_PASS");
            ConnectionString.Database = Utilities.Config.GetStringElement("WORLD_DATABASE_NAME");
            ConnectionString.IgnorePrepare = false;
            ConnectionString.MinimumPoolSize = 1;
            ConnectionString.MaximumPoolSize = 10;
        }

        public static MySqlConnection CreateConnection()
        {
            var connection = new MySqlConnection(ConnectionString.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
