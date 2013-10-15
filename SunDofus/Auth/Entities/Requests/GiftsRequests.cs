using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Timers;

namespace SunDofus.Auth.Entities.Requests
{
    class GiftsRequests
    {
        public static List<Models.GiftsModel> GetGiftsByAccountID(int accID)
        {
            var list = new List<Models.GiftsModel>();

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM gifts WHERE Target=@target";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@target", accID));

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var gift = new Models.GiftsModel()
                    {
                        ID = sqlReader.GetInt16("Id"),
                        Target = sqlReader.GetInt16("Target"),
                        ItemID = sqlReader.GetInt16("ItemID"),
                        Title = sqlReader.GetString("Title"),
                        Message = sqlReader.GetString("Message"),
                        Image = sqlReader.GetString("Image"),
                    };

                    list.Add(gift);
                }

                sqlReader.Close();
            }

            return list;
        }

        public static void DeleteGift(int giftID, int accountID)
        {
            if (accountID == -1)
                return;

            SunDofus.Auth.Entities.DatabaseProvider.CheckConnection();

            lock (SunDofus.Auth.Entities.DatabaseProvider.Locker)
            {
                var sqlText = "DELETE FROM gifts WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@id", giftID));

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
