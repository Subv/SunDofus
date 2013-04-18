using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Realm.World
{
    class Save
    {        
        public static void SaveWorld()
        {
            SaveChararacters();
        }

        public static void SaveChararacters()
        {
            Network.ServersHandler.AuthLinks.Send(new Network.Authentication.Packets.StartMaintenancePacket().GetPacket());

            Characters.CharactersManager.CharactersList.Where(x => x.isConnected).ToList().ForEach(x => x.NetworkClient.Send("Im1164"));

            foreach (var character in Characters.CharactersManager.CharactersList)
            {
                Entities.Cache.CharactersCache.SaveCharacter(character);
                System.Threading.Thread.Sleep(100);
            }

            Characters.CharactersManager.CharactersList.Where(x => x.isConnected).ToList().ForEach(x => x.NetworkClient.Send("Im1165"));

            Network.ServersHandler.AuthLinks.Send(new Network.Authentication.Packets.StopMaintenancePacket().GetPacket());
        }
    }
}
