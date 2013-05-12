using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.World.Conditions;

namespace SunDofus.World.Entities.Models.Maps
{
    class TriggerModel
    {
        private int _mapID, _cellID, _actionID;
        private string _args, _con;

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
        public int ActionID
        {
            get
            {
                return _actionID;
            }
            set
            {
                _actionID = value;
            }
        }

        public string Args
        {
            get
            {
                return _args;
            }
            set
            {
                _args = value;
            }
        }
        public string Conditions
        {
            get
            {
                return _con;
            }
            set
            {
                _con = value;
            }
        }

        public TriggerModel()
        {
            Conditions = "";
            MapID = -1;
        }
    }
}
