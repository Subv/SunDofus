using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Spells
{
    class SpellToLearnModel
    {
        private int _race, _lvl, _spellID, _pos;

        public int Race
        {
            get
            {
                return _race;
            }
            set
            {
                _race = value;
            }
        }
        public int Level
        {
            get
            {
                return _lvl;
            }
            set
            {
                _lvl = value;
            }
        }
        public int SpellID
        {
            get
            {
                return _spellID;
            }
            set
            {
                _spellID = value;
            }
        }
        public int Pos
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

        public SpellToLearnModel()
        {
            Race = 0;
            Level = 0;
            SpellID = 0;
            Pos = 0;
        }
    }
}
