using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.World.Entities;
using MySql.Data.MySqlClient;

namespace SunDofus.Databases
{
    enum Queries
    {
        InsertNewBank,
        UpdateBank,

        InsertNewGuild,
        UpdateGuild,
        DeleteGuild,

        InsertNewCollector,
        UpdateCollector,
        DeleteCollector,

        InsertNewCharacter,
        UpdateCharacter,
        DeleteCharacter
    }

    static class PreparedStatements
    {
        static Dictionary<Queries, MySqlCommand> QueryMap = new Dictionary<Queries, MySqlCommand>();

        public static void PrepareStatements()
        {
            QueryMap.Clear();

            MySqlCommand command = new MySqlCommand("INSERT INTO banks (Owner, Kamas, Items) VALUES (@Owner, @Kamas, @Items)", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.InsertNewBank, PrepareBank(command));

            command = new MySqlCommand("UPDATE banks SET Owner=@Owner, Kamas=@Kamas, Items=@Items WHERE Owner=@Owner", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.UpdateBank, PrepareBank(command));

            command = new MySqlCommand("INSERT INTO guilds(ID, Name, Emblem, Level, Exp, Members, Stats) VALUES (@ID, @Name, @Emblem, @Level, @Exp, @Members, @Stats)", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.InsertNewGuild, PrepareGuild(command));

            command = new MySqlCommand("UPDATE guilds SET ID=@ID, Name=@Name, Level=@Level, Exp=@Exp, Stats=@Stats, Members=@Members, Emblem=@Emblem WHERE ID=@ID", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.UpdateGuild, PrepareGuild(command));

            command = new MySqlCommand("DELETE FROM guilds WHERE ID=@ID", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.DeleteGuild, PrepareGuild(command));

            command = new MySqlCommand("INSERT INTO collectors(ID, OwnerID, Mappos, Name, GuildID) VALUES (@ID, @Owner, @Mappos, @Name, @GuildID)", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.InsertNewCollector, PrepareCollector(command));

            command = new MySqlCommand("UPDATE collectors SET ID=@ID, Name=@Name, OwnerID=@Owner, Mappos=@Mappos, GuildID=@GuildID WHERE ID=@ID", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.UpdateCollector, PrepareCollector(command));

            command = new MySqlCommand("DELETE FROM collectors WHERE ID=@ID", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.DeleteCollector, PrepareCollector(command));

            command = new MySqlCommand("INSERT INTO characters(id, name, level, class, sex, color, color2, color3, mappos, stats, items, spells, experience, faction, zaaps, savepos) VALUES (@id, @name, @level, @class, @sex, @color, @color2, @color3, @mapinfos, @stats, @items, @spells, @exp, @faction, @zaaps, @savepos)", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.InsertNewCharacter, PrepareCharacter(command));

            command = new MySqlCommand("UPDATE characters SET id=@id, name=@name, level=@level, class=@class, sex=@sex, color=@color, color2=@color2, color3=@color3, mappos=@mapinfos, stats=@stats, items=@items, spells=@spells, experience=@exp, faction=@faction, zaaps=@zaaps, savepos=@savepos WHERE id=@id", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.UpdateCharacter, PrepareCharacter(command));

            command = new MySqlCommand("DELETE FROM characters WHERE name=@name", DatabaseProvider.CreateConnection());
            QueryMap.Add(Queries.DeleteCharacter, PrepareCharacter(command));
        }

        static MySqlCommand PrepareBank(MySqlCommand command)
        {
            command.Parameters.Add("@Owner", MySqlDbType.Int32);
            command.Parameters.Add("@Kamas", MySqlDbType.Int64);
            command.Parameters.Add("@Items", MySqlDbType.Text);
            command.Prepare();
            return command;
        }

        static MySqlCommand PrepareGuild(MySqlCommand command)
        {
            command.Parameters.Add("@ID", MySqlDbType.Int32);
            command.Parameters.Add("@Name", MySqlDbType.Text);
            command.Parameters.Add("@Level", MySqlDbType.Int32);
            command.Parameters.Add("@Exp", MySqlDbType.Int64);
            command.Parameters.Add("@Stats", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@Members", MySqlDbType.Text);
            command.Parameters.Add("@Emblem", MySqlDbType.VarChar, 255);
            command.Prepare();
            return command;
        }

        static MySqlCommand PrepareCollector(MySqlCommand command)
        {
            command.Parameters.Add("@ID", MySqlDbType.Int32);
            command.Parameters.Add("@Name", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@Owner", MySqlDbType.Int32);
            command.Parameters.Add("@Mappos", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@GuildID", MySqlDbType.Int32);
            command.Prepare();
            return command;
        }

        static MySqlCommand PrepareCharacter(MySqlCommand command)
        {
            command.Parameters.Add("@id", MySqlDbType.Int32);
            command.Parameters.Add("@name", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@level", MySqlDbType.Int32);
            command.Parameters.Add("@class", MySqlDbType.Int32);
            command.Parameters.Add("@sex", MySqlDbType.Int32);
            command.Parameters.Add("@color", MySqlDbType.Int32);
            command.Parameters.Add("@color2", MySqlDbType.Int32);
            command.Parameters.Add("@color3", MySqlDbType.Int32);
            command.Parameters.Add("@mapinfos", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@stats", MySqlDbType.Text);
            command.Parameters.Add("@items", MySqlDbType.Text);
            command.Parameters.Add("@spells", MySqlDbType.Text);
            command.Parameters.Add("@exp", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@faction", MySqlDbType.Text);
            command.Parameters.Add("@zaaps", MySqlDbType.Text);
            command.Parameters.Add("@savepos", MySqlDbType.VarChar, 255);
            command.Prepare();
            return command;
        }

        public static MySqlCommand GetQuery(Queries query)
        {
            return QueryMap[query];
        }
    }
}
