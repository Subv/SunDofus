using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Auth.Packets
{
    class ClientDisconnectedPacket
    {
        public string GetPacket(string data)
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(90), data).ToString();
        }
    }
}
