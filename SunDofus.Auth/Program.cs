﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace auth
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "SunDofus.Auth | Shaak [c]";

            Utilities.Config.LoadConfiguration();
            Utilities.Loggers.InitialiseLoggers();

            Database.DatabaseHandler.InitialiseConnection();
            Database.Cache.ServersCache.ReloadCache();

            Network.ServersHandler.InitialiseServers();

            Database.Cache.GiftsCache.ReloadCache();
            Database.Cache.AccountsCache.ReloadCache();

            while (true)
                Console.ReadLine();
        }
    }
}
