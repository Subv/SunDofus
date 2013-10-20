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

    }

    static class PreparedStatements
    {
        static Dictionary<Queries, string> QueryMap = new Dictionary<Queries, string>();

        static PreparedStatements()
        {
            QueryMap.Add(Queries.InsertNewBank, "INSERT INTO banks (Owner, Kamas, Items) VALUES (@Owner, @Kamas, @Items)");
            QueryMap.Add(Queries.UpdateBank, "UPDATE banks SET Owner=@Owner, Kamas=@Kamas, Items=@Items WHERE Owner=@Owner");
        }

        public static string GetQuery(Queries query)
        {
            return QueryMap[query];
        }
    }
}
