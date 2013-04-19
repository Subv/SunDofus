using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities
{
    class DatabaseProvider
    {
        public static MySqlConnection Connection;
        public static object ConnectionLocker;

        public static void InitializeConnection()
        {
            Connection = new MySqlConnection();
            ConnectionLocker = new object();

            try
            {
                Connection.ConnectionString = string.Format("server={0};uid={1};pwd='{2}';database={3}", 
                    Utilities.Config.GetStringElement("World_Database_Server"), 
                    Utilities.Config.GetStringElement("World_Database_User"), 
                    Utilities.Config.GetStringElement("World_Database_Pass"), 
                    Utilities.Config.GetStringElement("World_Database_Name"));

                lock (ConnectionLocker)
                    Connection.Open();

                Utilities.Loggers.StatusLogger.Write("Connected to the World_Database !");
            }
            catch (Exception e)
            {
                Utilities.Loggers.ErrorsLogger.Write(string.Format("Can't connect to the database : {0}", e.ToString()));
            }
        }

        public static void Open()
        {
            lock (ConnectionLocker)
                Connection.Open();

            Utilities.Loggers.StatusLogger.Write("Reconnected to the World_Database !");
        }

        public static void Close()
        {
            lock (ConnectionLocker)
                Connection.Close();

            Utilities.Loggers.StatusLogger.Write("Diconnected from the World_Database !");
        }
    }
}
