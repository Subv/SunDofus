using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Auth.Network.Sync.Packets
{
    class TransferPacket
    {
        public string GetPacket(params object[] datas)
        {
            return new MasterPacket(Utilities.Basic.DeciToHex(70), datas).ToString();
        }
    }
}
