using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class MapsRequests
    {
        public static List<Realm.Maps.Map> MapsList = new List<Realm.Maps.Map>();

        public static void LoadMaps()
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM maps";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var map = new Models.Maps.MapModel()
                    {
                        ID = sqlReader.GetInt32("id"),
                        Date = sqlReader.GetString("date"),
                        Width = sqlReader.GetInt16("width"),
                        Height = sqlReader.GetInt16("heigth"),
                        Capabilities = sqlReader.GetInt16("capabilities"),
                        Mappos = sqlReader.GetString("mappos"),
                        MapData = sqlReader.GetString("mapData"),
                        Key = sqlReader.GetString("key"),
                        MaxMonstersGroup = sqlReader.GetInt16("numgroup"),
                        MaxGroupSize = sqlReader.GetInt16("groupsize"),
                    };

                    foreach (var newMonster in sqlReader.GetString("monsters").Split('|'))
                    {
                        if (newMonster == "")
                            continue;

                        var infos = newMonster.Split(',');

                        if (infos.Length < 2)
                            continue;
                        if (infos[1].Length < 1)
                            continue;

                        lock (map.Monsters)
                        {
                            if (!map.Monsters.ContainsKey(int.Parse(infos[0])))
                                map.Monsters.Add(int.Parse(infos[0]), new List<int>());
                        }

                        lock(map.Monsters[int.Parse(infos[0])])
                            map.Monsters[int.Parse(infos[0])].Add(int.Parse(infos[1]));
                    }

                    map.ParsePos();

                    lock(MapsList)
                        MapsList.Add(new Realm.Maps.Map(map));
                }

                sqlReader.Close();
            }

            Utilities.Loggers.StatusLogger.Write(string.Format("Loaded '{0}' maps from the database !", MapsList.Count));
        }
    }
}
