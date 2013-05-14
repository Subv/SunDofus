using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class TriggersRequests
    {
        public static List<Entities.Models.Maps.TriggerModel> TriggersList = new List<Entities.Models.Maps.TriggerModel>();

        public static void LoadTriggers()
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM datas_triggers";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var trigger = new Entities.Models.Maps.TriggerModel()
                    {
                        MapID = sqlReader.GetInt32("MapID"),
                        CellID = sqlReader.GetInt32("CellID"),
                        ActionID = sqlReader.GetInt16("ActionID"),
                        Args = sqlReader.GetString("Args"),
                        Conditions = sqlReader.GetString("Conditions"),
                    };                    

                    lock (TriggersList)
                    {
                        if(ParseTrigger(trigger))
                            TriggersList.Add(trigger);
                    }
                }

                sqlReader.Close();
            }

            Utilities.Loggers.StatusLogger.Write(string.Format("Loaded '{0}' triggers from the database !", TriggersList.Count));
        }

        public static void InsertTrigger(Models.Maps.TriggerModel trigger)
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "INSERT INTO datas_triggers VALUES(@mapid, @cellid, @action, @args, @condi)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@mapid", trigger.MapID));
                P.Add(new MySqlParameter("@cellid", trigger.CellID));
                P.Add(new MySqlParameter("@action", trigger.ActionID));
                P.Add(new MySqlParameter("@args", trigger.Args));
                P.Add(new MySqlParameter("@condi", trigger.Conditions));

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static bool ParseTrigger(Entities.Models.Maps.TriggerModel trigger)
        {
            if (MapsRequests.MapsList.Any(x => x.Model.ID == trigger.MapID))
            {
                MapsRequests.MapsList.First(x => x.Model.ID == trigger.MapID).Triggers.Add(trigger);
                return true;
            }
            else
                return false;
        }
    }
}
