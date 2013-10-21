using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using SunDofus.Databases;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class GuildsRequest
    {
        // We need to use a ConcurrentBag here as the Save thread will be accessing this list while we might still be writing to it.
        public static ConcurrentBag<Game.Guilds.Guild> GuildsList = new ConcurrentBag<Game.Guilds.Guild>();

        public static void LoadGuilds()
        {
            var connection = DatabaseProvider.CreateConnection();
            var sqlText = "SELECT * FROM guilds";
            var sqlCommand = new MySqlCommand(sqlText, connection);

            var sqlResult = sqlCommand.ExecuteReader();

            while (sqlResult.Read())
            {
                var guild = new Game.Guilds.Guild
                {
                    ID = sqlResult.GetInt32("ID"),
                    Name = sqlResult.GetString("Name"),
                    Level = sqlResult.GetInt32("Level"),
                    Exp = sqlResult.GetInt64("Exp"),

                    SaveState = EntityState.Unchanged
                };

                guild.ParseEmblem(sqlResult.GetString("Emblem"));
                guild.ParseSqlMembers(sqlResult.GetString("Members"));
                guild.ParseSqlStats(sqlResult.GetString("Stats"));

                GuildsList.Add(guild);
            }

            sqlResult.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' guilds from the database !", GuildsList.Count));
        }

        public static void SaveGuild(Game.Guilds.Guild guild)
        {
            if (guild.SaveState == EntityState.Unchanged)
                return;

            MySqlCommand command = null;
            if (guild.SaveState == EntityState.New)
                command = PreparedStatements.GetQuery(Queries.InsertNewGuild);
            else if (guild.SaveState == EntityState.Modified)
                command = PreparedStatements.GetQuery(Queries.UpdateGuild);
            else
                command = PreparedStatements.GetQuery(Queries.DeleteGuild);

            command.Parameters["@ID"].Value = guild.ID;
            command.Parameters["@Name"].Value = guild.Name;
            command.Parameters["@Level"].Value = guild.Level;
            command.Parameters["@Exp"].Value = guild.Exp;
            command.Parameters["@Stats"].Value = guild.GetSqlStats();
            command.Parameters["@Members"].Value = guild.GetSqlMembers();
            command.Parameters["@Emblem"].Value = guild.GetSqlEmblem();

            command.ExecuteNonQuery();
            
            guild.SaveState = EntityState.Unchanged;
        }
    }
}
