using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Fights
{
    class Fighter
    {
        public bool IsMonster { get; set; }
        public Characters.Character Player { get; set; }

        public int Team { get; set; }

        public Fighter(Characters.Character player, int team)
        {
            Player = player;
            Team = team;
        }
    }
}
