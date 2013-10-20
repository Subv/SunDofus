using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.Databases;

namespace SunDofus.World.Entities.Requests
{
    class BanksRequests
    {
        public static List<Game.Bank.Bank> BanksList = new List<Game.Bank.Bank>();

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
                        SaveState = EntityState.Unchanged
                    };

                    bank.ParseItems(sqlResult.GetString("Items"));

                    BanksList.Add(bank);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' banks from the database !", BanksList.Count));
        }

        public static void SaveBank(Game.Bank.Bank bank, ref MySqlCommand updateCommand, ref MySqlCommand createCommand)
        {
            if (bank.SaveState == EntityState.Unchanged)
                return;

            lock (DatabaseProvider.Locker)
            {
                MySqlCommand command = null;
                if (bank.SaveState == EntityState.New)
                {
                    if (createCommand == null)
                        createCommand = new MySqlCommand(PreparedStatements.GetQuery(Queries.InsertNewBank), DatabaseProvider.Connection);
                    command = createCommand;
                }
                else
                {
                    if (updateCommand == null)
                        updateCommand = new MySqlCommand(PreparedStatements.GetQuery(Queries.UpdateBank), DatabaseProvider.Connection);
                    command = updateCommand;
                }
                
                // Prepare the command for faster execution on further queries
                if (!command.IsPrepared)
                {
                    command.Parameters.Add("@Owner", MySqlDbType.Int32);
                    command.Parameters.Add("@Kamas", MySqlDbType.Int64);
                    command.Parameters.Add("@Items", MySqlDbType.Text);
                    command.Prepare();
                }

                command.Parameters["@Owner"].Value = bank.Owner;
                command.Parameters["@Kamas"].Value = bank.Kamas;
                command.Parameters["@Items"].Value = bank.GetItems();

                command.ExecuteNonQuery();
                
                bank.SaveState = EntityState.Unchanged;
            }
        }
    }
}
