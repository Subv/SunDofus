using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Auth.Packets
{
    class CreatedFriendPacket
    {
        public string GetPacket(params object[] datas)
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(130), datas).ToString();
        }
    }
}
