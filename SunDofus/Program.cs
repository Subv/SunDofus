using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Utilities;
using SunDofus.Auth.Entities;
using SunDofus.Auth.Entities.Requests;
using SunDofus.World.Network;
using System.Reflection;
using System.Threading;

namespace SunDofus
{
    class Program
    {
        static void Main(string[] args)
        {
            Basic.Uptime = Environment.TickCount;
            Console.Title = string.Concat("SunDofus v", Config.Version(Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim()));

            Config.LoadConfiguration();
            Loggers.InitializeLoggers();

            if (Config.GetBoolElement("REALM"))
            {
                var realmthread = new Thread(new ThreadStart(new Action(delegate()
                    {
                        try
                        {
                            Auth.Network.ServersHandler.InitialiseServers();
                            Auth.Entities.DatabaseProvider.InitializeConnection();

                            Loggers.Debug.Write(string.Format("Realm started in '{0}'s !", Basic.GetUpTime()[2]));
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine(error);
                        }
                    })));

                realmthread.Start();
            }

            if (Config.GetBoolElement("WORLD"))
            {
                var gamethread = new Thread(() =>
                    {
                        try
                        {
                            Console.Title = string.Format("{0} | Server '{1}'", Console.Title, Config.GetIntElement("ServerID"));

                            World.Entities.DatabaseProvider.Initialize();

                            World.Entities.Requests.LevelsRequests.LoadLevels();

                            World.Entities.Requests.ItemsRequests.LoadItems();
                            World.Entities.Requests.ItemsRequests.LoadItemsSets();
                            World.Entities.Requests.ItemsRequests.LoadUsablesItems();

                            World.Entities.Requests.SpellsRequests.LoadSpells();
                            World.Entities.Requests.SpellsRequests.LoadSpellsToLearn();

                            World.Entities.Requests.MonstersRequests.LoadMonsters();
                            World.Entities.Requests.MonstersRequests.LoadMonstersLevels();

                            World.Entities.Requests.MapsRequests.LoadMaps();

                            if(!Utilities.Config.GetBoolElement("DEBUG"))
                                World.Entities.Requests.TriggersRequests.LoadTriggers();

                            World.Entities.Requests.ZaapsRequests.LoadZaaps();
                            World.Entities.Requests.ZaapisRequests.LoadZaapis();

                            World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCsAnswers();
                            World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCsQuestions();
                            World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCs();

                            World.Entities.Requests.BanksRequests.LoadBanks();
                            World.Entities.Requests.CharactersRequests.LoadCharacters();
                            World.Entities.Requests.GuildsRequest.LoadGuilds();
                            World.Entities.Requests.CollectorsRequests.LoadCollectors();

                            World.Network.ServersHandler.InitialiseServers();

                            World.Entities.Requests.AuthsRequests.LoadAuths();

                            World.Game.World.Save.InitSaveThread();

                            Loggers.Debug.Write(string.Format("World started in '{0}'s !", Basic.GetUpTime()[2]));
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine(error);
                        }
                    });

                gamethread.Start();
            }

            while (true)
            {
                Console.ReadKey();
                Loggers.Debug.Write(string.Format("Uptime : Hours : {0} - Minutes : {1} - Seconds : {2}", 
                    Basic.GetUpTime()[0], Basic.GetUpTime()[1], Basic.GetUpTime()[2]));
            }
        }
    }
}
