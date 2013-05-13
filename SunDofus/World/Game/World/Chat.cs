﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters;

namespace SunDofus.World.Game.World
{
    class Chat
    {
        public static void SendGeneralMessage(Network.Realm.RealmClient client, string message)
        {
            if (client.Player.GetMap() == null) 
                return;

            if (message.Substring(0, 1) == ".")
            {
                client.Commander.ParseChatCommand(message.Substring(1));
                return;
            }

            client.Player.GetMap().Send(string.Format("cMK|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
        }

        public static void SendIncarnamMessage(Network.Realm.RealmClient client, string message)
        {
            if (!client.Player.isInIncarnam || client.Player.Level > 30)
            {
                client.Send("Im0139");
                return;
            }

            foreach (var character in Network.ServersHandler.RealmServer.Clients.Where
                (x => x.Authentified == true && x.Player.isInIncarnam))
            {
                character.Send(string.Format("cMK^|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
        }

        public static void SendPrivateMessage(Network.Realm.RealmClient client, string receiver, string message)
        {
            if (CharactersManager.CharactersList.Any(x => x.Name == receiver))
            {
                var character = CharactersManager.CharactersList.First(x => x.Name == receiver);

                if (character.isConnected == true && !character.NetworkClient.Enemies.Contains(client.Infos.Pseudo))
                {
                    character.NetworkClient.Send(string.Format("cMKF|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
                    client.Send(string.Format("cMKT|{0}|{1}|{2}", client.Player.ID, character.Name, message));
                }
                else
                    client.Send(string.Format("cMEf{0}", receiver));
            }
        }

        public static void SendTradeMessage(Network.Realm.RealmClient client, string message)
        {
            if (client.Player.CanSendinTrade() == true)
            {
                foreach (var character in Network.ServersHandler.RealmServer.Clients.Where(x => x.Authentified == true))
                    character.Send(string.Format("cMK:|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));

                client.Player.RefreshTrade();
            }
            else
                client.Send(string.Format("Im0115;{0}", client.Player.TimeTrade()));
        }

        public static void SendRecruitmentMessage(Network.Realm.RealmClient client, string message)
        {
            if (client.Player.CanSendinRecruitment() == true)
            {
                foreach (var character in Network.ServersHandler.RealmServer.Clients.Where(x => x.Authentified == true))
                    character.Send(string.Format("cMK?|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));

                client.Player.RefreshRecruitment();
            }
            else
                client.Send(string.Format("Im0115;{0}", client.Player.TimeRecruitment()));
        }

        public static void SendFactionMessage(Network.Realm.RealmClient client, string message)
        {
            if (client.Player.Faction.ID != 0 && client.Player.Faction.Level >= 3)
            {
                foreach (var character in Network.ServersHandler.RealmServer.Clients.Where(x => x.Authentified == true && x.Player.Faction.ID == client.Player.Faction.ID))
                    character.Send(string.Format("cMK!|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
        }

        public static void SendPartyMessage(Network.Realm.RealmClient client, string message)
        {
            if (client.Player.State.Party != null)
            {
                foreach (var character in client.Player.State.Party.Members.Keys)
                    character.NetworkClient.Send(string.Format("cMK$|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
        }

        public static void SendAdminMessage(Network.Realm.RealmClient client, string message)
        {
            if (client.Infos.Level > 0)
            {
                foreach (var character in Network.ServersHandler.RealmServer.Clients.Where(x => x.Authentified == true && x.Infos.Level > 0))
                    character.Send(string.Format("cMK@|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
        }
    }
}
