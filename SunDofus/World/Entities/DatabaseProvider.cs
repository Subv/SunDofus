using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities
{
    class DatabaseProvider
    {
        public static MySqlConnection Connection { get; set; }
        public static object Locker { get; set; }

        public static void InitializeConnection()
        {
            Connection = new MySqlConnection();
            Locker = new object();

            Connection.ConnectionString = string.Format("server={0};uid={1};pwd='{2}';database={3};IgnorePrepare=false",
                Utilities.Config.GetStringElement("WORLD_DATABASE_SERVER"),
                Utilities.Config.GetStringElement("WORLD_DATABASE_USER"),
                Utilities.Config.GetStringElement("WORLD_DATABASE_PASS"),
                Utilities.Config.GetStringElement("WORLD_DATABASE_NAME"));
            
            lock (Locker)
                Connection.Open();

            Utilities.Loggers.Status.Write("Connected to the Worlds' Database !");
        }

        public static void Open()
        {
            lock (Locker)
                Connection.Open();

            Utilities.Loggers.Status.Write("Reconnected to the Worlds' Database !");
        }

        public static void Close()
        {
            lock (Locker)
                Connection.Close();

            Utilities.Loggers.Status.Write("Diconnected from the Worlds 'Database !");
        }
    }
}
