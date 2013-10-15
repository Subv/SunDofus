using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Auth.Network.Sync.Packets
{
    class PHelloConnectSuccess
    {
        public string GetPacket()
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(30)).ToString();
        }
    }
}
