using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Zaaps
{
    class Zaap
    {
        private int _mapID, _cellID;

        public int MapID
        {
            get
            {
                return _mapID;
            }
            set
            {
                _mapID = value;
            }
        }
        public int CellID
        {
            get
            {
                return _cellID;
            }
            set
            {
                _cellID = value;
            }
        }

        public Map Map;
    }
}
