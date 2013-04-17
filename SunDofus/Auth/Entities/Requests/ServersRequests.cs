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
        private static List<Models.ServersModel> _servers;

        public static List<Models.ServersModel> Cache
        {
            get
            {
                return _servers;
            }
        }

        public static void LoadCache()
        {
            _servers = new List<Models.ServersModel>();

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM dyn_servers";
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

                    lock (_servers)
                        _servers.Add(server);
                }

                sqlReader.Close();
            }
        }
    }
}
