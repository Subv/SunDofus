using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SunDofus.World.Game.World
{
    class Save
    {        
        private static Timer _timer;
        public static void InitSaveThread()
        {
            _timer = new Timer((e) =>
            {
                SaveWorld();
                _timer.Change(1 * 60 * 1000, Timeout.Infinite);
            }, null, 1 * 60 * 1000, Timeout.Infinite);
        }

        public static void SaveWorld()
        {
            SaveChararacters();
        }

        public static void SaveChararacters()
        {
            Entities.DatabaseProvider.Open();
            Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.StartMaintenancePacket().GetPacket());
            Characters.CharactersManager.CharactersList.Where(x => x.isConnected).ToList().ForEach(x => x.NetworkClient.Send("Im1164"));

            foreach (var character in Characters.CharactersManager.CharactersList)
            {
                Entities.Requests.CharactersRequests.SaveCharacter(character);
                System.Threading.Thread.Sleep(100);
            }

            Characters.CharactersManager.CharactersList.Where(x => x.isConnected).ToList().ForEach(x => x.NetworkClient.Send("Im1165"));
            Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.StopMaintenancePacket().GetPacket());
            Entities.DatabaseProvider.Close();

            Utilities.Loggers.StatusLogger.Write("Save of the World successfull !");
        }
    }
}
