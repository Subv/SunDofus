using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MySql.Data.MySqlClient;

namespace SunDofus.Auth.Entities.Requests
{
    class ServersRequests
    {
        public static List<Models.ServersModel> Cache { get; set; }

        public static void LoadCache()
        {
            Cache = new List<Models.ServersModel>();

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM gameservers";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var server = new Models.ServersModel()
                    {
                        ID = sqlReader.GetInt16("Id"),
                        IP = sqlReader.GetString("Ip"),
                        Port = sqlReader.GetInt16("Port"),
                        PassKey = sqlReader.GetString("PassKey"),
                    };

                    lock (Cache)
                        Cache.Add(server);
                }

                sqlReader.Close();
            }
        }
    }
}
