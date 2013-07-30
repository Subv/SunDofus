using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class BanksRequests
    {
        public static List<Game.Bank.Bank> BanksList = new List<Game.Bank.Bank>();

        public static void LoadBanks()
        {
            lock (DatabaseProvider.ConnectionLocker)
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
                        isNewBank = false
                    };

                    bank.ParseItems(sqlResult.GetString("Items"));

                    BanksList.Add(bank);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.StatusLogger.Write(string.Format("Loaded '{0}' banks from the database !", BanksList.Count));
        }

        public static void SaveBank(Game.Bank.Bank bank)
        {
            if (bank.isNewBank)
            {
                CreateBank(bank);
                return;
            }
            else
            {
                lock (DatabaseProvider.ConnectionLocker)
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
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "INSERT INTO collectors VALUES(@Owner, @Kamas, @Items)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@Owner", bank.Owner));
                P.Add(new MySqlParameter("@Kamas", bank.Kamas));
                P.Add(new MySqlParameter("@Items", bank.GetItems()));

                sqlCommand.ExecuteNonQuery();

                bank.isNewBank = false;
            }
        }
    }
}
