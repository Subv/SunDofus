using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Spells
{
    class SpellModel
    {
        private int _ID, _sprite;
        private string _spriteinfos;

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
        public int Sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
            }
        }

        public string SpriteInfos
        {
            get
            {
                return _spriteinfos;
            }
            set
            {
                _spriteinfos = value;
            }
        }

        public List<SpellLevelModel> Levels;

        public SpellModel()
        {
            Levels = new List<SpellLevelModel>();
            ID = -1;
            Sprite = -1;
            SpriteInfos = "";
        }

        public void ParseLevel(string _datas)
        {
            if (_datas == "-1")
                return;

            var level = new SpellLevelModel();
            var stats = _datas.Split(',');

            var effect = stats[0];
            var effectCC = stats[1];

            if (stats[2] != "" && stats[2] != "-1")
                level.Cost = int.Parse(stats[2]);
            else
                level.Cost = 6;

            level.MinRP = int.Parse(stats[3]);
            level.MaxRP = int.Parse(stats[4]);
            level.CC = int.Parse(stats[5]);
            level.EC = int.Parse(stats[6]);

            level.isOnlyLine = (stats[7] == "true" ? true : false);
            level.isOnlyViewLine = (stats[8] == "true" ? true : false);
            level.isEmptyCell = (stats[9] == "true" ? true : false);
            level.isAlterablePO = (stats[10] == "true" ? true : false);

            level.MaxPerTurn = int.Parse(stats[12]);
            level.MaxPerPlayer = int.Parse(stats[13]);
            level.TurnNumber = int.Parse(stats[14]);
            level.Type = stats[15];
            level.isECEndTurn = (stats[19] == "true" ? true : false);

            level.ParseEffect(effect, false);
            level.ParseEffect(effectCC, true);

            Levels.Add(level);
        }
    }
}
