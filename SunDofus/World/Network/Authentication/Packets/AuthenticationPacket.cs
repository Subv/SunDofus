using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Authentication.Packets
{
    class AuthenticationPacket
    {
        public string GetPacket(params object[] datas)
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(20), datas).ToString();
        }
    }
}
