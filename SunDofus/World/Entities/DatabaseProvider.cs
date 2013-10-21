using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Databases;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities
{
    class DatabaseProvider
    {
        static MySqlConnectionStringBuilder ConnectionString;
        static MySqlConnection Connection; // Should never be closed, and is never accessed by two threads at the same time.

        public static void Initialize()
        {
            ConnectionString = new MySqlConnectionStringBuilder();
            ConnectionString.Server = Utilities.Config.GetStringElement("WORLD_DATABASE_SERVER");
            ConnectionString.UserID = Utilities.Config.GetStringElement("WORLD_DATABASE_USER");
            ConnectionString.Password = Utilities.Config.GetStringElement("WORLD_DATABASE_PASS");
            ConnectionString.Database = Utilities.Config.GetStringElement("WORLD_DATABASE_NAME");
            ConnectionString.IgnorePrepare = false;
            ConnectionString.Pooling = false;

            Connection = new MySqlConnection(ConnectionString.ConnectionString);
            Connection.Open();
        }

        public static MySqlConnection CreateConnection()
        {
            if (Connection.State == System.Data.ConnectionState.Broken)
            {
                // Re-establish the connection
                Connection.Close();
                Connection.Open();
                // Prepare all the statements again
                PreparedStatements.PrepareStatements();
            }

            return Connection;
        }
    }
}
