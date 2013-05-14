using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class AuthsRequests
    {
        public static void LoadAuths()
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM dyn_auths";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var server = new Models.Clients.AuthClientModel()
                    {
                        ID = sqlReader.GetInt16("Id"),
                        IP = sqlReader.GetString("Ip"),
                        Port = sqlReader.GetInt16("Port"),
                        PassKey = sqlReader.GetString("PassKey"),
                    };

                    Network.ServersHandler.AuthLinks.AddAuthClient(server);
                }

                sqlReader.Close();
            }
        }
    }
}
