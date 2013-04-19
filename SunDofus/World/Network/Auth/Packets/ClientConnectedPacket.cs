using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Auth.Packets
{
    class ClientConnectedPacket
    {
        public string GetPacket(string data)
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(80), data).ToString();
        }
    }
}
