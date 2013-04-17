using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Cache
{
    class AuthsCache
    {
        public static List<Models.Clients.AuthClientModel> AuthsList = new List<Models.Clients.AuthClientModel>();

        public static void ReloadAuths()
        {
            lock (DatabaseHandler.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM dyn_auths";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseHandler.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var server = new Models.Clients.AuthClientModel();

                    server.ID = sqlReader.GetInt16("Id");
                    server.IP = sqlReader.GetString("Ip");
                    server.Port = sqlReader.GetInt16("Port");
                    server.PassKey = sqlReader.GetString("PassKey");

                    lock (AuthsList)
                        AuthsList.Add(server);
                }

                sqlReader.Close();
            }
        }
    }
}
