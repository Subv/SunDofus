using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Monsters
{
    class MonsterLevelModel
    {
        private int _ID, _creID, _gradeID;
        private int _lvl, _ap, _mp, _life;
        private int _rNeutr, _rStr, _rInt, _rLuck, _rAg;
        private int _rpa, _rpm;
        private int _wisdom, _str, _int, _luck, _ag;
        private int _exp;

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
        public int CreatureID
        {
            get
            {
                return _creID;
            }
            set
            {
                _creID = value;
            }
        }
        public int GradeID
        {
            get
            {
                return _gradeID;
            }
            set
            {
                _gradeID = value;
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
        public int AP
        {
            get
            {
                return _ap;
            }
            set
            {
                _ap = value;
            }
        }
        public int MP
        {
            get
            {
                return _mp;
            }
            set
            {
                _mp = value;
            }
        }
        public int Life
        {
            get
            {
                return _life;
            }
            set
            {
                _life = value;
            }
        }

        public int RNeutral
        {
            get
            {
                return _rNeutr;
            }
            set
            {
                _rNeutr = value;
            }
        }
        public int RStrenght
        {
            get
            {
                return _rStr;
            }
            set
            {
                _str = value;
            }
        }
        public int RIntel
        {
            get
            {
                return _rInt;
            }
            set
            {
                _rInt = value;
            }
        }
        public int RLuck
        {
            get
            {
                return _rLuck;
            }
            set
            {
                _rLuck = value;
            }
        }
        public int RAgility
        {
            get
            {
                return _rAg;
            }
            set
            {
                _rAg = value;
            }
        }

        public int RPa
        {
            get
            {
                return _rpa;
            }
            set
            {
                _rpa = value;
            }
        }
        public int RPm
        {
            get
            {
                return _rpm;
            }
            set
            {
                _rpm = value;
            }
        }

        public int Wisdom
        {
            get
            {
                return _wisdom;
            }
            set
            {
                _wisdom = value;
            }
        }
        public int Strenght
        {
            get
            {
                return _str;
            }
            set
            {
               _str  = value;
            }
        }
        public int Intel
        {
            get
            {
                return _int;
            }
            set
            {
                _int = value;
            }
        }
        public int Luck
        {
            get
            {
                return _luck;
            }
            set
            {
                _luck = value;
            }
        }
        public int Agility
        {
            get
            {
                return _ag;
            }
            set
            {
                _ag = value;
            }
        }

        public int Exp
        {
            get
            {
                return _exp;
            }
            set
            {
                _exp = value;
            }
        }
        public List<Game.Characters.Spells.CharacterSpell> Spells;

        public MonsterLevelModel()
        {
            Spells = new List<Game.Characters.Spells.CharacterSpell>();
        }
    }
}
