using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Authentication.Packets
{
    class StartMaintenancePacket
    {
        public string GetPacket()
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(50)).ToString();
        }
    }
}
