using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.Databases;

namespace SunDofus.World.Entities.Requests
{
    class BanksRequests
    {
        // We need to use a ConcurrentBag here as the Save thread will be accessing this list while we might still be writing to it.
        public static ConcurrentBag<Game.Bank.Bank> BanksList = new ConcurrentBag<Game.Bank.Bank>();

        public static void LoadBanks()
        {
            var connection = DatabaseProvider.CreateConnection();
            var sqlText = "SELECT * FROM banks";
            var sqlCommand = new MySqlCommand(sqlText, connection);

            var sqlResult = sqlCommand.ExecuteReader();

            while (sqlResult.Read())
            {
                var bank = new Game.Bank.Bank()
                {
                    Owner = sqlResult.GetInt32("Owner"),
                    Kamas = sqlResult.GetInt64("Kamas"),
                    SaveState = EntityState.Unchanged
                };

                bank.ParseItems(sqlResult.GetString("Items"));

                BanksList.Add(bank);
            }

            sqlResult.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' banks from the database !", BanksList.Count));
        }

        public static void SaveBank(Game.Bank.Bank bank)
        {
            if (bank.SaveState == EntityState.Unchanged)
                return;

            MySqlCommand command = null;
            if (bank.SaveState == EntityState.New)
                command = PreparedStatements.GetQuery(Queries.InsertNewBank);
            else
                command = PreparedStatements.GetQuery(Queries.UpdateBank);

            command.Parameters["@Owner"].Value = bank.Owner;
            command.Parameters["@Kamas"].Value = bank.Kamas;
            command.Parameters["@Items"].Value = bank.GetItems();

            command.ExecuteNonQuery();

            bank.SaveState = EntityState.Unchanged;
        }
    }
}
