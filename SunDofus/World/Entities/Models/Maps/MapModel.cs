using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Maps
{
    class MapModel
    {
        public int ID;
        public int Date;
        public int Width;
        public int Height;
        public int Capabilities;

        public int PosX;
        public int PosY;
        public int SubArea;

        public string MapData;
        public string Key;
        public string Mappos;

        public int MaxMonstersGroup;
        public int MaxGroupSize;

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
