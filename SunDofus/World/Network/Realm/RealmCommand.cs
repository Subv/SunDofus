﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Realm
{
    class RealmCommand
    {
        public RealmClient Client;

        public RealmCommand(RealmClient client)
        {
            Client = client;
        }

        public void ParseCommand(string args)
        {
            try
            {
                var datas = args.Split(' ');

                if (Client.Infos.GMLevel > 0)
                {
                    switch (datas[0])
                    {
                        case "add":
                            ParseCommandAdd(datas);
                            break;

                        case "bug":
                            ParseCommandBug(datas);
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

        #region CommandInfos

        private void ParseCommandBug(string[] datas)
        {
            try
            {
                switch (datas[1])
                {
                    case "trigger":

                        if (!System.IO.Directory.Exists("./output/"))
                            System.IO.Directory.CreateDirectory("./output/");
                        if (!System.IO.Directory.Exists("./output/bugs/"))
                            System.IO.Directory.CreateDirectory("./output/bugs/");

                        var writer = new System.IO.StreamWriter("./output/bugs/triggers.txt", true);
                        writer.WriteLine(Client.Player.MapID+ "<>" + Client.Player.MapCell);
                        writer.Close();
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
                        Entities.Cache.TriggersCache.InsertTrigger(t);

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
                    SunDofus.World.Realm.World.Save.SaveWorld();
                    return;
                }

                switch (datas[1])
                {
                    case "all":
                        SunDofus.World.Realm.World.Save.SaveWorld();
                        Client.SendConsoleMessage("World saved !", 0);
                        break;

                    case "char":
                        SunDofus.World.Realm.World.Save.SaveChararacters();
                        Client.SendConsoleMessage("Characters saved !", 0);
                        break;

                    default:
                        SunDofus.World.Realm.World.Save.SaveWorld();
                        Client.SendConsoleMessage("World saved !", 0);
                        break;
                }
            }
            catch
            {
                Client.SendConsoleMessage("Cannot parse your AdminCommand !");
                Client.SendConsoleMessage("Use the command 'Help' for more informations !");
            }
        }

        private void ParseCommandItem(string[] datas)
        {
            try
            {
                var item = Entities.Cache.ItemsCache.ItemsList.First(x => x.ID == int.Parse(datas[1]));

                if (datas.Length == 2)
                {
                    var newItem = new SunDofus.World.Realm.Characters.Items.CharacterItem(item);
                    newItem.GeneratItem();

                    Client.Player.ItemsInventary.AddItem(newItem, false);
                    Client.SendConsoleMessage("Item Added !", 0);
                }

                else if (datas.Length == 3)
                {
                    var newItem = new SunDofus.World.Realm.Characters.Items.CharacterItem(item);
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
                    var myMap = Entities.Cache.MapsCache.MapsList.First(x => x.GetModel.PosX == int.Parse(datas[1]) && x.GetModel.PosY == int.Parse(datas[2]));
                    Client.Player.TeleportNewMap(myMap.GetModel.ID, int.Parse(datas[3]));
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
