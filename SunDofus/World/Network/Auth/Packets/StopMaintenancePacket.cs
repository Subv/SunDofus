using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Auth.Packets
{
    class StopMaintenancePacket
    {
        public string GetPacket()
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(60)).ToString();
        }
    }
}
