using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class ZaapsRequests
    {
        public static List<World.Game.Maps.Zaaps.Zaap> ZaapsList = new List<Game.Maps.Zaaps.Zaap>();

        public static void LoadZaaps()
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM zaaps";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var zaap = new World.Game.Maps.Zaaps.Zaap()
                    {
                        MapID = sqlReader.GetInt32("mapID"),
                        CellID = sqlReader.GetInt32("cellID"),
                    };

                    lock (ZaapsList)
                    {
                        if (ParseZaap(zaap))
                            ZaapsList.Add(zaap);
                    }
                }

                sqlReader.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' zaaps from the database !", ZaapsList.Count));
        }

        private static bool ParseZaap(World.Game.Maps.Zaaps.Zaap zaap)
        {
            if (MapsRequests.MapsList.Any(x => x.Model.ID == zaap.MapID) && !ZaapsList.Any(x => x.MapID == zaap.MapID))
            {
                zaap.Map = MapsRequests.MapsList.First(x => x.Model.ID == zaap.MapID);
                return true;
            }
            else
                return false;
        }
    }
}
