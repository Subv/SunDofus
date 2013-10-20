using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        static Dictionary<Queries, string> QueryMap = new Dictionary<Queries, string>();

        static PreparedStatements()
        {
            QueryMap.Add(Queries.InsertNewBank, "INSERT INTO banks (Owner, Kamas, Items) VALUES (@Owner, @Kamas, @Items)");
            QueryMap.Add(Queries.UpdateBank, "UPDATE banks SET Owner=@Owner, Kamas=@Kamas, Items=@Items WHERE Owner=@Owner");

            QueryMap.Add(Queries.InsertNewGuild, "INSERT INTO guilds(ID, Name, Emblem, Level, Exp, Members, Stats) VALUES (@ID, @Name, @Emblem, @Level, @Exp, @Members, @Stats)");
            QueryMap.Add(Queries.UpdateGuild, "UPDATE guilds SET ID=@ID, Name=@Name, Level=@Level, Exp=@Exp, Stats=@Stats, Members=@Members, Emblem=@Emblem WHERE ID=@ID");
            QueryMap.Add(Queries.DeleteGuild, "DELETE FROM guilds WHERE ID=@ID");

            QueryMap.Add(Queries.InsertNewCollector, "INSERT INTO collectors(ID, OwnerID, Mappos, Name, GuildID) VALUES (@ID, @Owner, @Mappos, @Name, @GuildID)");
            QueryMap.Add(Queries.UpdateCollector, "UPDATE collectors SET ID=@ID, Name=@Name, OwnerID=@Owner, Mappos=@Mappos, GuildID=@GuildID WHERE ID=@ID");
            QueryMap.Add(Queries.DeleteCollector, "DELETE FROM collectors WHERE ID=@ID");

            QueryMap.Add(Queries.InsertNewCharacter, "INSERT INTO characters(id, name, level, class, sex, color, color2, color3, mappos, stats, items, spells, experience, faction, zaaps, savepos) VALUES (@id, @name, @level, @class, @sex, @color, @color2, @color3, @mapinfos, @stats, @items, @spells, @exp, @faction, @zaaps, @savepos)");
            QueryMap.Add(Queries.UpdateCharacter, "UPDATE characters SET id=@id, name=@name, level=@level, class=@class, sex=@sex, color=@color, color2=@color2, color3=@color3, mappos=@mapinfos, stats=@stats, items=@items, spells=@spells, experience=@exp, faction=@faction, zaaps=@zaaps, savepos=@savepos WHERE id=@id");
            QueryMap.Add(Queries.DeleteCharacter, "DELETE FROM characters WHERE name=@name");
        }

        public static string GetQuery(Queries query)
        {
            return QueryMap[query];
        }
    }
}
