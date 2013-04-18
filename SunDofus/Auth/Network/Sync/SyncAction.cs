using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Auth.Network.Sync
{
    class SyncAction
    {
        public static void UpdateCharacters(int accountID, string character, int serverID, bool add = true)
        {
            if (accountID == -1)
                return;

            lock (SunDofus.Auth.Entities.DatabaseProvider.ConnectionLocker)
            {
                if (add)
                {
                    var sqlText = "INSERT INTO dyn_accounts_char VALUES (@charname, @server, @account)";
                    var sqlCommand = new MySqlCommand(sqlText, SunDofus.Auth.Entities.DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@charname", character));
                    sqlCommand.Parameters.Add(new MySqlParameter("@server", serverID));
                    sqlCommand.Parameters.Add(new MySqlParameter("@account", accountID));

                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    var sqlText = "DELETE FROM dyn_accounts_char WHERE characterName=@charname";
                    var sqlCommand = new MySqlCommand(sqlText, SunDofus.Auth.Entities.DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@charname", character));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateConnectedValue(int accountID, bool isConnected)
        {
            if (accountID == -1)
                return;

            lock (SunDofus.Auth.Entities.DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "UPDATE dyn_accounts SET connected=@connected WHERE Id=@id";
                var sqlCommand = new MySqlCommand(sqlText, SunDofus.Auth.Entities.DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@id", accountID));
                sqlCommand.Parameters.Add(new MySqlParameter("@connected", (isConnected ? 1 : 0)));

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static void DeleteGift(int giftID, int accountID)
        {
            if (accountID == -1)
                return;

            lock (SunDofus.Auth.Entities.DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "DELETE FROM dyn_gifts WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, SunDofus.Auth.Entities.DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@id", giftID));

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
