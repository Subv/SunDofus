﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Timers;

namespace SunDofus.Entities.Requests
{
    class GiftsRequests
    {
        public static List<Models.GiftsModel> GetGiftsByAccountID(int accID)
        {
            var list = new List<Models.GiftsModel>();

            DatabaseProvider.CheckConnection();

            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = string.Format("SELECT * FROM dyn_gifts WHERE Target={0}", accID);
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

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
    }
}
