﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Realm.Characters.Spells
{
    class CharacterSpell
    {
        private int _ID;

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

        private int _level;

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

        private int _pos;

        public int Position
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
            }
        }

        public CharacterSpell(int id, int lvl, int pos)
        {
            _ID = id;
            _level = lvl;
            _pos = pos;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", _ID, _level, _pos);
        }
    }
}
