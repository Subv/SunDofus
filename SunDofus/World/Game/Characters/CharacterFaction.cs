using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterFaction
    {
        private int _ID, _honor, _deshonor;

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        public int Honor
        {
            get
            {
                return _honor;
            }
            set
            {
                _honor = value;
            }
        }
        public int Deshonor
        {
            get
            {
                return _deshonor;
            }
            set
            {
                _deshonor = value;
            }
        }

        public bool isEnabled = false;
    }
}
