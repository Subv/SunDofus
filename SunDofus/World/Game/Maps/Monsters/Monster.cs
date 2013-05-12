using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Monsters
{
    class Monster
    {
        public SunDofus.World.Entities.Models.Monsters.MonsterModel Model;

        private int _level;

        public int Level
        {
            get
            {
                return _level;
            }
        }

        public Monster(Entities.Models.Monsters.MonsterModel model, int grade)
        {
            Model = model;
            _level = grade;
        }
    }
}
