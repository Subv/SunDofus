﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SunDofus.World.Network.Realm
{
    class RealmClientCommander
    {
        public RealmClient Client;

        public RealmClientCommander(RealmClient client)
        {
            Client = client;
        }

        public void ParseAdminCommand(string args)
        {
            try
            {
                var datas = args.Split(' ');

                if (Client.Infos.Level > 0)
                {
                    switch (datas[0])
                    {
                        case "add":
                            ParseCommandAdd(datas);
                            break;

                        case "save":
                            ParseCommandSave(datas);
                            break;

                        case "vita":
                            ParseCommandVita(datas);
                            break;

                        case "item":
                            ParseCommandItem(datas);
                            break;

                        case "teleport":
                            ParseCommandTeleport(datas);
                            break;

                        case "faction":
                            ParseCommandFaction(datas);
                            break;

                        default:
                            Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                            Client.SendConsoleMessage("Use the command 'Help' for more informations !");
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                Client.SendConsoleMessage("Use the command 'Help' for more informations !");

                Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot parse command from <{0}> because : {1}", Client.myIp(), e.ToString()));
            }
        }

        public void ParseChatCommand(string args)
        {
            try
            {
                var datas = args.Split(' ');

                switch (datas[0])
                {
                    case "infos":
                        var uptime = Utilities.Basic.GetUpTime();
                        Client.SendMessage(string.Concat("SunDofus v<b>", Utilities.Config.Version(), "</b> <i>par Ghost</i>"));
                        Client.SendMessage(string.Concat("Nombre de joueurs connectés : <b>'", Network.ServersHandler.RealmServer.Clients.Count(x => x.Authentified), "'</b>"));
                        Client.SendMessage(string.Concat("Uptime : Heures <b>'", uptime[0], "'</b> Minutes <b>'", uptime[1],
                            "'</b> Secondes <b>'", uptime[2], "'</b>"));
                        break;

                    case "guild":
                        Client.Send("gn");
                        break;

                    case "bank":
                        Game.Bank.BankManager.OpenBank(Client.Player);
                        break;

                    default:
                        Client.SendConsoleMessage("Cannot parse your ChatCommand !");
                        break;
                }
            }
            catch (Exception e)
            {
                Client.SendMessage("Cannot parse your ChatCommand !");
                Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot parse command from <{0}> because : {1}", Client.myIp(), e.ToString()));
            }
        }

        #region CommandInfos

        private void ParseCommandFaction(string[] datas)
        {
            try
            {
                switch (datas[1])
                {
                    case "enabled":
                        
                        Client.Player.Faction.isEnabled = bool.Parse(datas[2].Trim());
                        Client.Player.SendChararacterStats();
                        Client.Player.TeleportNewMap(Client.Player.MapID, Client.Player.MapCell);
                        break;

                    case "change":
                        
                        Client.Player.Faction.ID = int.Parse(datas[2].Trim());
                        Client.Player.Faction.Level = 1;
                        Client.Player.Faction.Honor = 0;
                        Client.Player.Faction.Deshonor = 0;
                        Client.Player.Faction.isEnabled = false;
                        Client.Player.SendChararacterStats();
                        break;

                    case "addhonor":

                        Client.Player.Faction.Honor += int.Parse(datas[2].Trim());

                        if (Client.Player.Faction.Honor > Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Alignment).ToArray()[0].Alignment)
                            Client.Player.Faction.Level = 10;
                        else
                            Client.Player.Faction.Level = Entities.Requests.LevelsRequests.LevelsList.Where(x => x.Alignment <= Client.Player.Faction.Honor).OrderByDescending(x => x.Alignment).ToArray()[0].ID;

                        Client.Player.SendChararacterStats();
                        break;

                    case "adddeshonor":
                        
                        Client.Player.Faction.Deshonor += int.Parse(datas[2].Trim());
                        Client.Player.SendChararacterStats();
                        break;
                }
            }
            catch
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
            }
        }

        private void ParseCommandAdd(string[] datas)
        {
            try
            {
                var value = (long)0;
                if (!long.TryParse(datas[2], out value))
                {
                    Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                    return;
                }

                switch (datas[1])
                {
                    case "kamas":

                        Client.Player.Kamas += value;
                        Client.SendConsoleMessage("Kamas Added", 0);
                        Client.Player.SendChararacterStats();
                        break;

                    case "exp":

                        Client.Player.AddExp(value);
                        Client.SendConsoleMessage("Exp Added !", 0);
                        break;

                    case "trigger":

                        var t = new Entities.Models.Maps.TriggerModel();
                        t.ActionID = 0;
                        t.CellID = Client.Player.MapCell;
                        t.MapID = Client.Player.MapID;
                        t.Conditions = "";
                        t.Args = string.Format("{0},{1}", value, int.Parse(datas[3]));

                        Client.Player.GetMap().Triggers.Add(t);
                        Entities.Requests.TriggersRequests.InsertTrigger(t);

                        Client.SendConsoleMessage("Trigger Added !", 0);
                        break;

                    case "cellfight":

                        break;
                }
            }
            catch
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
            }
        }

        private void ParseCommandSave(string[] datas)
        {
            try
            {
                if (datas.Length <= 1)
                {
                    SunDofus.World.Game.World.Save.SaveWorld();
                    return;
                }

                switch (datas[1])
                {
                    case "all":
                        SunDofus.World.Game.World.Save.SaveWorld();
                        Client.SendConsoleMessage("World saved !", 0);
                        break;

                    default:
                        SunDofus.World.Game.World.Save.SaveWorld();
                        Client.SendConsoleMessage("World saved !", 0);
                        break;
                }
            }
            catch (Exception ex)
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                Client.SendConsoleMessage("Use the command 'Help' for more informations !");
                Client.SendConsoleMessage(string.Concat("Sources : ", ex.ToString()));
            }
        }

        private void ParseCommandItem(string[] datas)
        {
            try
            {
                var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == int.Parse(datas[1]));

                if (datas.Length == 2)
                {
                    var newItem = new SunDofus.World.Game.Characters.Items.CharacterItem(item);
                    newItem.GeneratItem();

                    Client.Player.ItemsInventary.AddItem(newItem, false);
                    Client.SendConsoleMessage("Item Added !", 0);
                }

                else if (datas.Length == 3)
                {
                    var newItem = new SunDofus.World.Game.Characters.Items.CharacterItem(item);
                    newItem.GeneratItem(int.Parse(datas[2]));

                    Client.Player.ItemsInventary.AddItem(newItem, false);
                    Client.SendConsoleMessage("Item Added !", 0);
                }

                else
                    Client.SendConsoleMessage("Invalid Syntax !");
            }
            catch
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                Client.SendConsoleMessage("Use the command 'Help' for more informations !");
            }
        }

        private void ParseCommandTeleport(string[] datas)
        {
            try
            {
                if (datas.Length == 3)
                {
                    Client.Player.TeleportNewMap(int.Parse(datas[1]), int.Parse(datas[2]));
                    Client.SendConsoleMessage("Character Teleported !", 0);
                }

                else if (datas.Length == 4)
                {
                    var myMap = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.PosX == int.Parse(datas[1]) && x.Model.PosY == int.Parse(datas[2]));
                    Client.Player.TeleportNewMap(myMap.Model.ID, int.Parse(datas[3]));
                    Client.SendConsoleMessage("Character Teleported !", 0);
                }

                else
                    Client.SendConsoleMessage("Invalid Syntax !");
            }
            catch(Exception e)
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                Client.SendConsoleMessage("Use the command 'Help' for more informations !");
                Client.SendConsoleMessage(e.ToString());
            }
        }

        private void ParseCommandVita(string[] datas)
        {
            try
            {
                if (datas.Length == 2)
                {
                    Client.Player.ResetVita(datas[1]);
                    Client.SendConsoleMessage("Vita Updated !", 0);
                }

                else
                    Client.SendConsoleMessage("Invalid Syntax !");
            }
            catch
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                Client.SendConsoleMessage("Use the command 'Help' for more informations !");
            }
        }

        #endregion

    }
}
