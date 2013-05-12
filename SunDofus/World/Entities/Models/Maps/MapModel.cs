using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Maps
{
    class MapModel
    {
        private int _ID, _width, _heigth, _cap;
        private int _posX, _posY, _subArea;
        private string _date, _mapData, _key, _mappons;
        private int _maxMon, _maxGroup;

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
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }
        public int Height
        {
            get
            {
                return _heigth;
            }
            set
            {
                _heigth = value;
            }
        }
        public int Capabilities
        {
            get
            {
                return _cap;
            }
            set
            {
                _cap = value;
            }
        }

        public int PosX
        {
            get
            {
                return _posX;
            }
            set
            {
                _posX = value;
            }
        }
        public int PosY
        {
            get
            {
                return _posY;
            }
            set
            {
                _posY = value;
            }
        }
        public int SubArea
        {
            get
            {
                return _subArea;
            }
            set
            {
                _subArea = value;
            }
        }

        public string MapData
        {
            get
            {
                return _mapData;
            }
            set
            {
                _mapData = value;
            }
        }
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        public string Mappos
        {
            get
            {
                return _mappons;
            }
            set
            {
                _mappons = value;
            }
        }

        public int MaxMonstersGroup
        {
            get
            {
                return _maxMon;
            }
            set
            {
                _maxMon = value;
            }
        }
        public int MaxGroupSize
        {
            get
            {
                return _maxGroup;
            }
            set
            {
                _maxGroup = value;
            }
        }

        public Dictionary<int, List<int>> Monsters;

        public MapModel()
        {
            MapData = "";
            Key = "";
            Mappos = "";

            Monsters = new Dictionary<int, List<int>>();
        }

        public void ParsePos()
        {
            var datas = Mappos.Split(',');
            try
            {
                PosX = int.Parse(datas[0]);
                PosY = int.Parse(datas[1]);
                SubArea = int.Parse(datas[2]);
            }
            catch { }
        }
    }
}
