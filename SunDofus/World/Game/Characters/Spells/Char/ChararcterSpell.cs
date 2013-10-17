using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Entities.Models.Spells;
using SunDofus.World.Entities.Requests;
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

            Model = SpellsRequests.SpellsList.First(x => x.ID == ID);
            LevelModel = Model.Levels.First(x => x.Level == Level);
        }

        public void ChangeLevel(int lvl)
        {
            Level = lvl;
            LevelModel = Model.Levels.First(x => x.Level == Level);
        }

        public SpellModel Model;
        public SpellLevelModel LevelModel;

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", ID, Level, Position);
        }
    }
}
