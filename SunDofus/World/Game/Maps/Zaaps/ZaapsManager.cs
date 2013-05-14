using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Zaaps
{
    class ZaapsManager
    {
        public static void SendZaaps(Characters.Character character)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.MapID))
            {
                var zaapis = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                if (!character.Zaaps.Contains(zaapis.MapID))
                {
                    character.Zaaps.Add(zaapis.MapID);
                    character.NetworkClient.Send("Im024");
                }

                var savepos = (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.SaveMap) ?
                    Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.SaveMap).MapID.ToString() : "");
                var packet = string.Concat("WC", savepos, "|");

                Entities.Requests.ZaapsRequests.ZaapsList.Where(x => character.Zaaps.Contains(x.MapID)).ToList().
                    ForEach(x => packet = string.Concat(packet, x.MapID, ";", CalcPrice(character.MapID, x.MapID), "|"));

                character.NetworkClient.Send(packet.Substring(0, packet.Length - 1));
            }
            else
                character.NetworkClient.Send("BN");
        }

        public static void SaveZaap(Characters.Character character)
        {
        }

        private static int CalcPrice(int startMap, int nextMap)
        {
            return 100;
        }
    }
}
