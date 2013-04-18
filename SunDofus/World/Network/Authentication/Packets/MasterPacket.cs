using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Authentication.Packets
{
    class MasterPacket
    {
        private string _header;
        private object[] _params;

        private bool _hadParams;

        public MasterPacket(string header)
        {
            _header = header;
            _hadParams = false;
        }

        public MasterPacket(string header, params object[] datas)
        {
            _header = header;
            _params = datas;
            _hadParams = true;
        }

        public override string ToString()
        {
            if (_hadParams)
                return string.Format("{0}|{1}", _header, string.Join("|", _params));
            else
                return _header;
        }
    }
}
