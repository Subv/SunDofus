﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Game.World
{
    class Save
    {        
        private static Timer timer;

        public static void InitSaveThread()
        {
            timer = new Timer((e) =>
            {
                SaveWorld();
                timer.Change(Utilities.Config.GetIntElement("AUTOSAVETIME") * 60 * 1000, Timeout.Infinite);
            }, null, Utilities.Config.GetIntElement("AUTOSAVETIME") * 60 * 1000, Timeout.Infinite);
        }

        public static void SaveWorld()
        {
            Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.StartMaintenancePacket().GetPacket());
            SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Where(x => x.IsConnected).ToList().ForEach(x => x.NClient.Send("Im1164"));

            SaveChararacters();
            SaveGuilds();
            SaveCollectors();
            SaveBanks();

            SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Where(x => x.IsConnected).ToList().ForEach(x => x.NClient.Send("Im1165"));
            Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.StopMaintenancePacket().GetPacket());

            Utilities.Loggers.Status.Write("Save of the World successfully !");
        }

        private static void SaveChararacters()
        {
            MySqlCommand create = null;
            MySqlCommand update = null;
            MySqlCommand delete = null;
            foreach (var character in SunDofus.World.Entities.Requests.CharactersRequests.CharactersList)
                Entities.Requests.CharactersRequests.SaveCharacter(character, ref create, ref update, ref delete);
        }

        private static void SaveGuilds()
        {
            MySqlCommand create = null;
            MySqlCommand update = null;
            MySqlCommand delete = null;
            foreach (var guild in Entities.Requests.GuildsRequest.GuildsList)
                Entities.Requests.GuildsRequest.SaveGuild(guild, ref create, ref update, ref delete);
        }

        private static void SaveCollectors()
        {
            MySqlCommand create = null;
            MySqlCommand update = null;
            MySqlCommand delete = null;
            foreach (var collector in Entities.Requests.CollectorsRequests.CollectorsList)
                Entities.Requests.CollectorsRequests.SaveCollector(collector, ref create, ref update, ref delete);
        }

        private static void SaveBanks()
        {
            MySqlCommand update = null;
            MySqlCommand create = null;
            foreach (var bank in Entities.Requests.BanksRequests.BanksList)
                Entities.Requests.BanksRequests.SaveBank(bank, ref update, ref create);
        }
    }
}
