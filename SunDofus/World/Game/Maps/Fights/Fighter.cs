using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Fights
{
    class Fighter
    {
        public bool isMonster = false;
        public Characters.Character Player;

        public int _team;

        public int Team
        {
            get
            {
                return _team;
            }
        }

        public Fighter(Characters.Character player, int team)
        {
            Player = player;
            _team = team;
        }
    }
}
