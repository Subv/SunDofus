﻿using System;
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
            Models.AccountsModel account = null;

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {

                var sqlText = "SELECT * FROM accounts WHERE username=@username";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@username", username));

                var sqlReader = sqlCommand.ExecuteReader();

                if (sqlReader.Read())
                {
                    account = new Models.AccountsModel()
                    {
                        ID = sqlReader.GetInt16("id"),
                        Username = sqlReader.GetString("username"),
                        Password = sqlReader.GetString("password"),
                        Pseudo = sqlReader.GetString("pseudo"),
                        Communauty = sqlReader.GetInt16("communauty"),
                        Level = sqlReader.GetInt16("gmLevel"),
                        Question = sqlReader.GetString("question"),
                        Answer = sqlReader.GetString("answer"),
                        SubscriptionDate = sqlReader.GetDateTime("subscription"),
                    };
                }

                sqlReader.Close();

                if (account != null)
                {
                    account.Characters = LoadCharacters(account.ID);
                    account.Friends = LoadFriends(account.ID);
                    account.Enemies = LoadEnemies(account.ID);
                }
            }

            return account;
        }

        public static Models.AccountsModel LoadAccount(int accountID)
        {
            Models.AccountsModel account = null;

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {

                var sqlText = "SELECT * FROM accounts WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);
                sqlCommand.Parameters.Add(new MySqlParameter("@id", accountID));

                var sqlReader = sqlCommand.ExecuteReader();

                if (sqlReader.Read())
                {
                    account = new Models.AccountsModel()
                    {
                        ID = sqlReader.GetInt16("id"),
                        Username = sqlReader.GetString("username"),
                        Password = sqlReader.GetString("password"),
                        Pseudo = sqlReader.GetString("pseudo"),
                        Communauty = sqlReader.GetInt16("communauty"),
                        Level = sqlReader.GetInt16("gmLevel"),
                        Question = sqlReader.GetString("question"),
                        Answer = sqlReader.GetString("answer"),
                        SubscriptionDate = sqlReader.GetDateTime("subscription"),
                    };
                }

                sqlReader.Close();

                if (account != null)
                {
                    account.Characters = LoadCharacters(account.ID);
                    account.Friends = LoadFriends(account.ID);
                    account.Enemies = LoadEnemies(account.ID);
                }
            }

            return account;
        }

        public static Dictionary<int, List<string>> LoadCharacters(int accID)
        {
            var dico = new Dictionary<int, List<string>>();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT serverID, characterName FROM accounts_characters WHERE accountID=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);
                sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
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

        public static List<string> LoadFriends(int accID)
        {
            var friends = new List<string>();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT targetPseudo FROM accounts_friends WHERE accID=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);
                sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    if (!friends.Contains(sqlReader.GetString("targetPseudo")))
                        friends.Add(sqlReader.GetString("targetPseudo"));
                }

                sqlReader.Close();
            }

            return friends;
        }

        public static List<string> LoadEnemies(int accID)
        {
            var enemies = new List<string>();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT targetPseudo FROM accounts_enemies WHERE accID=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);
                sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    if (!enemies.Contains(sqlReader.GetString("targetPseudo")))
                        enemies.Add(sqlReader.GetString("targetPseudo"));
                }

                sqlReader.Close();
            }

            return enemies;
        }

        public static int GetAccountID(string pseudo)
        {
            var accountID = -1;

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT id FROM accounts WHERE pseudo=@pseudo";
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

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "UPDATE accounts SET connected=0";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static void UpdateFriend(int accID, string targetPseudo, bool add)
        {
            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                if (add)
                {
                    var sqlText = "INSERT INTO accounts_friends VALUES (@id, @pseudo)";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));
                    sqlCommand.Parameters.Add(new MySqlParameter("@pseudo", targetPseudo));

                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    var sqlText = "DELETE FROM accounts_friends WHERE accID=@id AND targetPseudo=@pseudo";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));
                    sqlCommand.Parameters.Add(new MySqlParameter("@pseudo", targetPseudo));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateEnemy(int accID, string targetPseudo, bool add)
        {
            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                if (add)
                {
                    var sqlText = "INSERT INTO accounts_enemies VALUES (@id, @pseudo)";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));
                    sqlCommand.Parameters.Add(new MySqlParameter("@pseudo", targetPseudo));

                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    var sqlText = "DELETE FROM accounts_enemies WHERE accID=@id AND targetPseudo=@pseudo";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@id", accID));
                    sqlCommand.Parameters.Add(new MySqlParameter("@pseudo", targetPseudo));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        public static void UpdateCharacters(int accountID, string character, int serverID, bool add = true)
        {
            if (accountID == -1)
                return;

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                if (add)
                {
                    var sqlText = "INSERT INTO accounts_characters VALUES (@charname, @server, @account)";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@charname", character));
                    sqlCommand.Parameters.Add(new MySqlParameter("@server", serverID));
                    sqlCommand.Parameters.Add(new MySqlParameter("@account", accountID));

                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    var sqlText = "DELETE FROM accounts_characters WHERE characterName=@charname";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    sqlCommand.Parameters.Add(new MySqlParameter("@charname", character));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateConnectedValue(int accountID, bool isConnected)
        {
            if (accountID == -1)
                return;

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "UPDATE accounts SET connected=@connected WHERE Id=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@id", accountID));
                sqlCommand.Parameters.Add(new MySqlParameter("@connected", (isConnected ? 1 : 0)));

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
