using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Timers;

namespace SunDofus.Auth.Entities.Requests
{
    class AccountsRequests
    {
        public static Models.AccountsModel LoadAccount(string username)
        {
            var account = new Models.AccountsModel();

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.ConnectionLocker)
            {

                var sqlText = "SELECT * FROM dyn_accounts WHERE username=@username";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@username", username));

                var sqlReader = sqlCommand.ExecuteReader();

                if (sqlReader.Read())
                {
                    account.ID = sqlReader.GetInt16("id");
                    account.Username = sqlReader.GetString("username");
                    account.Password = sqlReader.GetString("password");
                    account.Pseudo = sqlReader.GetString("pseudo");
                    account.Communauty = sqlReader.GetInt16("communauty");
                    account.Level = sqlReader.GetInt16("gmLevel");
                    account.Question = sqlReader.GetString("question");
                    account.Answer = sqlReader.GetString("answer");
                    account.SubscriptionDate = sqlReader.GetDateTime("subscription");
                }

                sqlReader.Close();

                account.Characters = LoadCharacters(account.ID);
            }

            return account;
        }

        public static Models.AccountsModel LoadAccount(int accountID)
        {
            var account = new Models.AccountsModel();

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.ConnectionLocker)
            {

                var sqlText = "SELECT * FROM dyn_accounts WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);
                sqlCommand.Parameters.Add(new MySqlParameter("@id", accountID));

                var sqlReader = sqlCommand.ExecuteReader();

                if (sqlReader.Read())
                {
                    account.ID = sqlReader.GetInt16("id");
                    account.Username = sqlReader.GetString("username");
                    account.Password = sqlReader.GetString("password");
                    account.Pseudo = sqlReader.GetString("pseudo");
                    account.Communauty = sqlReader.GetInt16("communauty");
                    account.Level = sqlReader.GetInt16("gmLevel");
                    account.Question = sqlReader.GetString("question");
                    account.Answer = sqlReader.GetString("answer");
                    account.SubscriptionDate = sqlReader.GetDateTime("subscription");
                }

                sqlReader.Close();

                account.Characters = LoadCharacters(account.ID);
            }

            return account;
        }

        public static Dictionary<int, List<string>> LoadCharacters(int accID)
        {
            var dico = new Dictionary<int, List<string>>();

            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT serverID, characterName FROM dyn_accounts_char WHERE accountID=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);
                sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));

                var sqlReader = sqlCommand.ExecuteReader();

                if (sqlReader.Read())
                {
                    var serverID = sqlReader.GetInt16("serverID");
                    var charName = sqlReader.GetString("characterName");

                    if (!dico.ContainsKey(serverID))
                        dico.Add(serverID, new List<string>());

                    if (!dico[serverID].Contains(charName))
                        dico[serverID].Add(charName);
                }

                sqlReader.Close();
            }

            return dico;
        }

        public static int GetAccountID(string pseudo)
        {
            var accountID = -1;

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT id FROM dyn_accounts WHERE pseudo=@pseudo";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@pseudo", pseudo));

                var sqlReader = sqlCommand.ExecuteReader();

                if (sqlReader.Read())
                    accountID = sqlReader.GetInt32("id");

                sqlReader.Close();
            }

            return accountID;
        }

        public static void ResetConnectedValue()
        {
            DatabaseProvider.CheckConnection();

            lock (Entities.DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "UPDATE dyn_accounts SET connected=@connected";
                var sqlCommand = new MySqlCommand(sqlText, Entities.DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@connected", 0));

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
