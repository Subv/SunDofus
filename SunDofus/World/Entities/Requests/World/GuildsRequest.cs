using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class GuildsRequest
    {
        public static List<Game.Guilds.Guild> GuildsList = new List<Game.Guilds.Guild>();

        public static void LoadGuilds()
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM guilds";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlResult = sqlCommand.ExecuteReader();

                while (sqlResult.Read())
                {
                    var guild = new Game.Guilds.Guild
                    {
                        ID = sqlResult.GetInt32("ID"),
                        Name = sqlResult.GetString("Name"),
                        Level = sqlResult.GetInt32("Level"),
                        Exp = sqlResult.GetInt64("Exp"),

                        isNewGuild = false,
                    };

                    guild.ParseEmblem(sqlResult.GetString("Emblem"));
                    guild.ParseSqlMembers(sqlResult.GetString("Members"));
                    guild.ParseSqlStats(sqlResult.GetString("Stats"));

                    GuildsList.Add(guild);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.StatusLogger.Write(string.Format("Loaded '{0}' guilds from the database !", GuildsList.Count));
        }

        public static void SaveGuild(Game.Guilds.Guild guild)
        {
            if (guild.isNewGuild && !guild.mustDelete)
            {
                CreateGuild(guild);
                return;
            }
            else if (guild.mustDelete)
            {
                DeleteGuild(guild.ID);
                return;
            }
            else if (!guild.mustDelete && !guild.isNewGuild)
            {
                lock (DatabaseProvider.ConnectionLocker)
                {
                    var sqlText = "UPDATE guild SET ID=@ID, Name=@Name, Level=@Level, Exp=@Exp, Stats=@Stats," +
                        " Members=@Members, Emblem=@Emblem WHERE ID=@ID";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                    var P = sqlCommand.Parameters;
                    P.Add(new MySqlParameter("@ID", guild.ID));
                    P.Add(new MySqlParameter("@Name", guild.Name));
                    P.Add(new MySqlParameter("@Level", guild.Level));
                    P.Add(new MySqlParameter("@Exp", guild.Exp));
                    P.Add(new MySqlParameter("@Stats", guild.GetSqlStats()));
                    P.Add(new MySqlParameter("@Members", guild.GetSqlMembers()));
                    P.Add(new MySqlParameter("@Emblem", guild.GetSqlEmblem()));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private static void DeleteGuild(int guildID)
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "DELETE FROM guilds WHERE ID=@ID";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@ID", guildID));

                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void CreateGuild(Game.Guilds.Guild guild)
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "INSERT INTO guilds VALUES(@ID, @Name, @Emblem, @Level, @Exp, @Stats, @Members)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@ID", guild.ID));
                P.Add(new MySqlParameter("@Name", guild.Name));
                P.Add(new MySqlParameter("@Level", guild.Level));
                P.Add(new MySqlParameter("@Exp", guild.Exp));
                P.Add(new MySqlParameter("@Stats", guild.GetSqlStats()));
                P.Add(new MySqlParameter("@Members", guild.GetSqlMembers()));
                P.Add(new MySqlParameter("@Emblem", guild.GetSqlEmblem()));
                
                sqlCommand.ExecuteNonQuery();

                guild.isNewGuild = false;
            }
        }
    }
}
