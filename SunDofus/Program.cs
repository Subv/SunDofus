﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Utilities;
using SunDofus.Auth.Entities;
using SunDofus.Auth.Entities.Requests;
using SunDofus.World.Network;

namespace SunDofus
{
    class Program
    {
        static void Main(string[] args)
        {
            Basic.Uptime = Environment.TickCount;
            Console.Title = "SunDofus";

            Config.LoadConfiguration();
            Loggers.InitializeLoggers();

            if (Config.GetBoolElement("Realm"))
            {
                try
                {
                    Auth.Network.ServersHandler.InitialiseServers();
                    Auth.Entities.DatabaseProvider.InitializeConnection();

                }
                catch (Exception error)
                {
                    Console.WriteLine(error);
                }
            }

            if (Config.GetBoolElement("World"))
            {
                try
                {
                    Console.Title = string.Format("{0} | Server '{1}'", Console.Title, Config.GetIntElement("ServerID"));

                    World.Entities.DatabaseProvider.InitializeConnection();

                    World.Entities.Requests.LevelsRequests.LoadLevels();

                    World.Entities.Requests.ItemsRequests.LoadItems();
                    World.Entities.Requests.ItemsRequests.LoadItemsSets();
                    World.Entities.Requests.ItemsRequests.LoadUsablesItems();

                    World.Entities.Requests.SpellsRequests.LoadSpells();
                    World.Entities.Requests.SpellsRequests.LoadSpellsToLearn();

                    World.Entities.Requests.MonstersRequests.LoadMonsters();
                    World.Entities.Requests.MonstersRequests.LoadMonstersLevels();

                    World.Entities.Requests.MapsRequests.LoadMaps();
                    World.Entities.Requests.TriggersRequests.LoadTriggers();

                    World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCsAnswers();
                    World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCsQuestions();
                    World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCs();

                    World.Entities.Requests.CharactersRequests.LoadCharacters();

                    World.Network.ServersHandler.InitialiseServers();

                    World.Entities.Requests.AuthsRequests.LoadAuths();
                    World.Entities.DatabaseProvider.Close();
                }
                catch (Exception error)
                {
                    Console.WriteLine(error);
                }
            }

            Loggers.InfosLogger.Write(string.Format("Started in '{0}'ms !", Basic.Uptime));
            Console.ReadLine();
        }
    }
}
