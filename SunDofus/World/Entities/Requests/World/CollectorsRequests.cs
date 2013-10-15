using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class CollectorsRequests
    {
        public static List<Game.Guilds.GuildCollector> CollectorsList = new List<Game.Guilds.GuildCollector>();

        public static void LoadCollectors()
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM collectors";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlResult = sqlCommand.ExecuteReader();

                while (sqlResult.Read())
                {
                    var mappos = sqlResult.GetString("Mappos").Split(';');
                    var map = MapsRequests.MapsList.First(x => x.Model.ID == int.Parse(mappos[0]));
                    var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == sqlResult.GetInt32("OwnerID"));

                    var collector = new Game.Guilds.GuildCollector(map, character, sqlResult.GetInt32("ID"))
                    {
                        IsNewCollector = false
                    };

                    collector.Name[0] = int.Parse(sqlResult.GetString("Name").Split(';')[0]);
                    collector.Name[1] = int.Parse(sqlResult.GetString("Name").Split(';')[1]);

                    collector.Map = map;
                    collector.Cell = int.Parse(mappos[1]);
                    collector.Dir = int.Parse(mappos[2]);

                    if (character.Guild == null || character.Guild.ID != sqlResult.GetInt32("GuildID"))
                        collector.Guild = GuildsRequest.GuildsList.First(x => x.ID == sqlResult.GetInt32("GuildID"));

                    collector.Guild.Collectors.Add(collector);
                    CollectorsList.Add(collector);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' collectors from the database !", CollectorsList.Count));
        }

        public static void SaveCollector(Game.Guilds.GuildCollector collector)
        {
            if (collector.IsNewCollector && !collector.MustDelete)
            {
                CreateCollector(collector);
                return;
            }
            else if (collector.MustDelete)
            {
                DeleteCollector(collector.ID);
                return;
            }
            else if (!collector.MustDelete && !collector.IsNewCollector)
            {
                lock (DatabaseProvider.Locker)
                {
                    var sqlText = "UPDATE collectors SET ID=@ID, Name=@Name, OwnerID=@Owner, Mappos=@Mappos, GuildID=@GuildID WHERE ID=@ID";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    var P = sqlCommand.Parameters;

                    P.Add(new MySqlParameter("@ID", collector.ID));
                    P.Add(new MySqlParameter("@Name", string.Join(";", collector.Name)));
                    P.Add(new MySqlParameter("@Owner", collector.Owner));
                    P.Add(new MySqlParameter("@Mappos", string.Concat(collector.Map.Model.ID, ";", collector.Cell, ";", collector.Dir)));
                    P.Add(new MySqlParameter("@GuildID", collector.Guild.ID));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private static void DeleteCollector(int collectorID)
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "DELETE FROM collectors WHERE ID=@ID";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@ID", collectorID));

                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void CreateCollector(Game.Guilds.GuildCollector collector)
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "INSERT INTO collectors VALUES(@ID, @Owner, @Mappos, @Name, @GuildID)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@ID", collector.ID));
                P.Add(new MySqlParameter("@Name", string.Join(";", collector.Name)));
                P.Add(new MySqlParameter("@Owner", collector.Owner));
                P.Add(new MySqlParameter("@Mappos", string.Concat(collector.Map.Model.ID, ";", collector.Cell, ";", collector.Dir)));
                P.Add(new MySqlParameter("@GuildID", collector.Guild.ID));

                sqlCommand.ExecuteNonQuery();

                collector.IsNewCollector = false;
            }
        }
    }
}
