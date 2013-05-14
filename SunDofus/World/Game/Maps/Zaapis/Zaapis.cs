using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Zaapis
{
    class Zaapis
    {
        private int _mapID, _faction, _cellID;

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
        public int Faction
        {
            get
            {
                return _faction;
            }
            set
            {
                _faction = value;
            }
        }

        public Map Map;
    }
}
