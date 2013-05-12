using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterFaction
    {
        private int _ID, _honor, _deshonor, _level;

        private Character character;

        public CharacterFaction(Character _character)
        {
            character = _character;
        }

        public bool isEnabled = false;

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
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        public string AlignementInfos
        {
            get
            {
                return string.Concat(_ID, ",", _ID, ",", (isEnabled ? _level.ToString() : "0"), ",", (character.Level + character.ID));
            }
        }

        public override string ToString()
        {
            return string.Concat(_ID, "~2,", _level, ",", _level, ",", _honor, ",", _deshonor, ",", (isEnabled ? "1" : "0"));
        }
    }
}
