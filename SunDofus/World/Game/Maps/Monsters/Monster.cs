using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Monsters
{
    class Monster
    {
        public SunDofus.World.Entities.Models.Monsters.MonsterModel Model { get; set; }

        public int Level { get; set; }

        public Monster(Entities.Models.Monsters.MonsterModel model, int grade)
        {
            Model = model;
            Level = grade;
        }
    }
}
