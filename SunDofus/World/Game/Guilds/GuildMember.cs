using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Guilds
{
    class GuildMember
    {
        private int _rank;
        private int _expGaved;
        private int _expGived;
        private int _rights;

        public int Rank
        {
            get
            {
                return _rank;
            }
            set
            {
                _rank = value;
            }
        }
        public int ExpGaved
        {
            get
            {
                return _expGaved;
            }
            set
            {
                _expGaved = value;
            }
        }
        public int ExpGived
        {
            get
            {
                return _expGived;
            }
            set
            {
                _expGived = value;
            }
        }
        public int Rights
        {
            get
            {
                return _rights;
            }
            set
            {
                _rights = value;
            }
        }

        public Characters.Character Character;

        public GuildMember(Characters.Character _character)
        {
            Character = _character;
        }

        public string SqlToString()
        {
            return string.Concat(Character.ID, ";", Rank, ";", ExpGaved, ";", ExpGived, ";", Rights);
        }
    }
}
