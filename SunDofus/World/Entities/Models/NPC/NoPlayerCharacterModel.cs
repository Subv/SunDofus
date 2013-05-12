using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.NPC
{
    class NoPlayerCharacterModel
    {
        private int _ID, _gfxID, _size, _sex;
        private int _col, _col2, _col3;

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
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
               _size  = value;
            }
        }
        public int Sex
        {
            get
            {
                return _sex;
            }
            set
            {
                _sex = value;
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

        //public int ArtWork;
        //public int Bonus;

        public NPCsQuestion Question;

        public string Name;
        public string Items;

        public List<int> SellingList;

        public NoPlayerCharacterModel()
        {
            SellingList = new List<int>();
            Question = null;
        }
    }
}
