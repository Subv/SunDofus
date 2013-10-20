using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Databases;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class GuildsRequest
    {
        public static List<Game.Guilds.Guild> GuildsList = new List<Game.Guilds.Guild>();

        public static void LoadGuilds()
        {
            using (var connection = DatabaseProvider.CreateConnection())
            {
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
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' guilds from the database !", GuildsList.Count));
        }

        public static void SaveGuild(Game.Guilds.Guild guild, ref MySqlCommand create, ref MySqlCommand update, ref MySqlCommand delete)
        {
            if (guild.SaveState == EntityState.Unchanged)
                return;

            using (var connection = DatabaseProvider.CreateConnection())
            {
                MySqlCommand command = null;
                if (guild.SaveState == EntityState.New)
                {
                    if (create == null)
                        create = new MySqlCommand(PreparedStatements.GetQuery(Queries.InsertNewGuild), connection);
                    command = create;
                }
                else if (guild.SaveState == EntityState.Modified)
                {
                    if (update == null)
                        update = new MySqlCommand(PreparedStatements.GetQuery(Queries.UpdateGuild), connection);
                    command = update;
                }
                else
                {
                    if (delete == null)
                        delete = new MySqlCommand(PreparedStatements.GetQuery(Queries.DeleteGuild), connection);
                    command = delete;
                }

                // Prepare the command for faster execution on further queries
                if (!command.IsPrepared)
                {
                    command.Parameters.Add("@ID", MySqlDbType.Int32);
                    command.Parameters.Add("@Name", MySqlDbType.Text);
                    command.Parameters.Add("@Level", MySqlDbType.Int32);
                    command.Parameters.Add("@Exp", MySqlDbType.Int64);
                    command.Parameters.Add("@Stats", MySqlDbType.VarChar, 255);
                    command.Parameters.Add("@Members", MySqlDbType.Text);
                    command.Parameters.Add("@Emblem", MySqlDbType.VarChar, 255);
                    command.Prepare();
                }

                command.Parameters["@ID"].Value = guild.ID;
                command.Parameters["@Name"].Value = guild.Name;
                command.Parameters["@Level"].Value = guild.Level;
                command.Parameters["@Exp"].Value = guild.Exp;
                command.Parameters["@Stats"].Value = guild.GetSqlStats();
                command.Parameters["@Members"].Value = guild.GetSqlMembers();
                command.Parameters["@Emblem"].Value = guild.GetSqlEmblem();

                command.ExecuteNonQuery();
            }
            
            guild.SaveState = EntityState.Unchanged;
        }
    }
}
