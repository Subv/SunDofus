using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class BanksRequests
    {
        // We need to use a ConcurrentBag here as the Save thread will be accessing this list while we might still be writing to it.
        public static ConcurrentBag<Game.Bank.Bank> BanksList = new ConcurrentBag<Game.Bank.Bank>();

        public static void LoadBanks()
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM banks";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlResult = sqlCommand.ExecuteReader();

                while (sqlResult.Read())
                {
                    var bank = new Game.Bank.Bank()
                    {
                        Owner = sqlResult.GetInt32("Owner"),
                        Kamas = sqlResult.GetInt64("Kamas"),
                        IsNewBank = false
                    };

                    bank.ParseItems(sqlResult.GetString("Items"));

                    BanksList.Add(bank);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' banks from the database !", BanksList.Count));
        }

        public static void SaveBank(Game.Bank.Bank bank)
        {
            if (bank.IsNewBank)
            {
                CreateBank(bank);
                return;
            }
            else
            {
                lock (DatabaseProvider.Locker)
                {
                    var sqlText = "UPDATE banks SET Owner=@Owner, Kamas=@Kamas, Items=@Items WHERE Owner=@Owner";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    var P = sqlCommand.Parameters;

                    P.Add(new MySqlParameter("@Owner", bank.Owner));
                    P.Add(new MySqlParameter("@Kamas", bank.Kamas));
                    P.Add(new MySqlParameter("@Items", bank.GetItems()));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private static void CreateBank(Game.Bank.Bank bank)
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "INSERT INTO banks VALUES(@Owner, @Kamas, @Items)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@Owner", bank.Owner));
                P.Add(new MySqlParameter("@Kamas", bank.Kamas));
                P.Add(new MySqlParameter("@Items", bank.GetItems()));

                sqlCommand.ExecuteNonQuery();

                bank.IsNewBank = false;
            }
        }
    }
}
