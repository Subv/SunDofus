using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Monsters
{
    class MonsterModel
    {
        private int _ID, _gfxID, _align, _col, _col2, _col3, _IA;
        private int _max, _min;
        private string _name;

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
        public int GfxID
        {
            get
            {
                return _gfxID;
            }
            set
            {
                _gfxID = value;
            }
        }
        public int Align
        {
            get
            {
                return _align;
            }
            set
            {
                _align = value;
            }
        }
        public int Color
        {
            get
            {
                return _col;
            }
            set
            {
                _col = value;
            }
        }
        public int Color2
        {
            get
            {
                return _col2;
            }
            set
            {
                _col2 = value;
            }
        }
        public int Color3
        {
            get
            {
                return _col3;
            }
            set
            {
                _col3 = value;
            }
        }
        public int IA
        {
            get
            {
                return _IA;
            }
            set
            {
                _IA = value;
            }
        }

        public int Max_kamas
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
        public int Min_kamas
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public List<MonsterLevelModel> Levels;
        public List<MonsterItem> Items;

        public MonsterModel()
        {
            Levels = new List<MonsterLevelModel>();
            Items = new List<MonsterItem>();
        }

        public class MonsterItem
        {
            public int ID;
            public double Chance;
            public int Max;

            public MonsterItem(int newID, double newChance, int newMax)
            {
                ID = newID;
                Chance = newChance;
                Max = newMax;
            }
        }
    }
}
