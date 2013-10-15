using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Guilds
{
    class GuildMember
    {
        public int Rank { get; set; }
        public int ExpGaved { get; set; }
        public int ExpGived { get; set; }
        public int Rights { get; set; }

        public Characters.Character Character { get; set; }

        public GuildMember(Characters.Character character)
        {
            Character = character;
        }

        public string SqlToString()
        {
            return string.Format("{0};{1};{2};{3};{4}", Character.ID, Rank, ExpGaved, ExpGived, Rights);
        }
    }
}
