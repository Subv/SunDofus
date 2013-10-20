using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class ZaapisRequests
    {
        public static List<World.Game.Maps.Zaapis.Zaapis> ZaapisList = new List<Game.Maps.Zaapis.Zaapis>();

        public static void LoadZaapis()
        {
            using (var connection = DatabaseProvider.CreateConnection())
            {
                var sqlText = "SELECT * FROM zaapis";
                var sqlCommand = new MySqlCommand(sqlText, connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var zaapis = new World.Game.Maps.Zaapis.Zaapis()
                    {
                        MapID = sqlReader.GetInt32("mapid"),
                        CellID = sqlReader.GetInt32("cellid"),
                        Faction = sqlReader.GetInt32("zone"),
                    };

                    if (ParseZaapis(zaapis))
                        ZaapisList.Add(zaapis);
                }

                sqlReader.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' zaapis from the database !", ZaapisList.Count));
        }

        private static bool ParseZaapis(World.Game.Maps.Zaapis.Zaapis zaapis)
        {
            if (MapsRequests.MapsList.Any(x => x.Model.ID == zaapis.MapID) && !ZaapisList.Any(x => x.MapID == zaapis.MapID))
            {
                zaapis.Map = MapsRequests.MapsList.First(x => x.Model.ID == zaapis.MapID);
                return true;
            }
            else
                return false;
        }
    }
}
