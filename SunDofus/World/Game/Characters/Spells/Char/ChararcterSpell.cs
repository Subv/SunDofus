using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters.Spells
{
    class CharacterSpell
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public int Position { get; set; }

        public CharacterSpell(int id, int lvl, int pos)
        {
            ID = id;
            Level = lvl;
            Position = pos;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", ID, Level, Position);
        }
    }
}
