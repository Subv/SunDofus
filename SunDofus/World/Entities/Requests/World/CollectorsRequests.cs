﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.Databases;

namespace SunDofus.World.Entities.Requests
{
    class CollectorsRequests
    {
        // We need to use a ConcurrentBag here as the Save thread will be accessing this list while we might still be writing to it.
        public static ConcurrentBag<Game.Guilds.GuildCollector> CollectorsList = new ConcurrentBag<Game.Guilds.GuildCollector>();

        public static void LoadCollectors()
        {
            using (var connection = DatabaseProvider.CreateConnection())
            {
                var sqlText = "SELECT * FROM collectors";
                var sqlCommand = new MySqlCommand(sqlText, connection);

                var sqlResult = sqlCommand.ExecuteReader();

                while (sqlResult.Read())
                {
                    var mappos = sqlResult.GetString("Mappos").Split(';');
                    var map = MapsRequests.MapsList.First(x => x.Model.ID == int.Parse(mappos[0]));
                    var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == sqlResult.GetInt32("OwnerID"));

                    var collector = new Game.Guilds.GuildCollector(map, character, sqlResult.GetInt32("ID"))
                    {
                        SaveState = EntityState.Unchanged
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

        public static void SaveCollector(Game.Guilds.GuildCollector collector, ref MySqlCommand create, ref MySqlCommand update, ref MySqlCommand delete)
        {
            if (collector.SaveState == EntityState.Unchanged)
                return;

            using (var connection = DatabaseProvider.CreateConnection())
            {
                MySqlCommand command = null;
                if (collector.SaveState == EntityState.New)
                {
                    if (create == null)
                        create = new MySqlCommand(PreparedStatements.GetQuery(Queries.InsertNewCollector), connection);
                    command = create;
                }
                else if (collector.SaveState == EntityState.Modified)
                {
                    if (update == null)
                        update = new MySqlCommand(PreparedStatements.GetQuery(Queries.UpdateCollector), connection);
                    command = update;
                }
                else
                {
                    if (delete == null)
                        delete = new MySqlCommand(PreparedStatements.GetQuery(Queries.DeleteCollector), connection);
                    command = delete;
                }

                if (!command.IsPrepared)
                {
                    command.Parameters.Add("@ID", MySqlDbType.Int32);
                    command.Parameters.Add("@Name", MySqlDbType.VarChar, 255);
                    command.Parameters.Add("@Owner", MySqlDbType.Int32);
                    command.Parameters.Add("@Mappos", MySqlDbType.VarChar, 255);
                    command.Parameters.Add("@GuildID", MySqlDbType.Int32);
                    command.Prepare();
                }

                command.Parameters["@ID"].Value = collector.ID;
                command.Parameters["@Name"].Value = string.Join(";", collector.Name);
                command.Parameters["@Owner"].Value = collector.Owner;
                command.Parameters["@Mappos"].Value = string.Concat(collector.Map.Model.ID, ";", collector.Cell, ";", collector.Dir);
                command.Parameters["@GuildID"].Value = collector.Guild.ID;

                command.ExecuteNonQuery();
            }
        }
    }
}
