using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.World.Conditions
{
    class NPCConditions
    {
        private int _condiID;
        private string _args;

        public int CondiID
        {
            get
            {
                return _condiID;
            }
            set
            {
                _condiID = value;
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

        public bool HasCondition(Game.Characters.Character character)
        {
            return true;
        }
    }
}
