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
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                if (!character.Zaaps.Contains(zaap.MapID))
                {
                    character.Zaaps.Add(zaap.MapID);
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
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.MapID))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                character.SaveMap = zaap.MapID;
                character.SaveCell = zaap.CellID;

                character.NetworkClient.Send("Im06");
            }
            else
                character.NetworkClient.Send("BN");
        }

        public static void OnMove(Characters.Character character, int nextZaap)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == nextZaap))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == nextZaap);

                var price = CalcPrice(character.MapID, nextZaap);

                character.Kamas -= price;
                character.NetworkClient.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaap.MapID, zaap.CellID);

                character.NetworkClient.Send("WV");

                character.SendChararacterStats();
            }
            else
                character.NetworkClient.Send("BN");
        }

        private static int CalcPrice(int startMap, int nextMap)
        {
            return 100;
        }
    }
}
