﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using SunDofus.Database.Models.NPC;

namespace SunDofus.Realm.Characters.NPC
{
    class NPCMap
    {
        private int _ID;

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

        private int _mapID;

        public int MapID
        {
            get
            {
                return _mapID;
            }
            set
            {
                _mapID = value;
            }
        }

        private int _mapCell;

        public int MapCell
        {
            get
            {
                return _mapCell;
            }
            set
            {
                _mapCell = value;
            }
        }

        private int _dir;

        public int Dir
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }

        public bool mustMove;

        public NoPlayerCharacterModel Model;
        private Timer _movements;

        public NPCMap(NoPlayerCharacterModel model)
        {
            Model = model;
        }

        public void StartMove()
        {
            if (mustMove == false || !Utilities.Config.GetConfig.GetBoolElement("MustNPCsMove"))
                return;

            _movements = new Timer();
            _movements.Enabled = true;
            _movements.Interval = Utilities.Basic.Rand(1000, 5000);
            _movements.Elapsed += new ElapsedEventHandler(this.Move);
        }

        public string PatternOnMap()
        {
            var builder = new StringBuilder();
            {
                builder.Append(MapCell).Append(";").Append(Dir).Append(";0;");
                builder.Append(ID).Append(";");
                builder.Append(Model.ID).Append(";-4;");
                builder.Append(Model.GfxID).Append("^").Append(Model.Size).Append(";");
                builder.Append(Model.Sex).Append(";").Append(Utilities.Basic.DeciToHex(Model.Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Model.Color2)).Append(";").Append(Utilities.Basic.DeciToHex(Model.Color3)).Append(";");
                builder.Append(Model.Items).Append(";;");
            }

            return builder.ToString();
        }

        private void Move(object e, EventArgs e2)
        {
            lock (Database.Cache.MapsCache.MapsList)
            {
                _movements.Interval = Utilities.Basic.Rand(1000, 5000);

                var map = Database.Cache.MapsCache.MapsList.First(x => x.GetModel.ID == MapID);

                var path = new Realm.Maps.Pathfinding("", map, MapCell, Dir);
                var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
                var newCell = path.NextCell(MapCell, newDir);

                if (newCell <= 0)
                    return;

                path.UpdatePath(Realm.Maps.Pathfinding.GetDirChar(Dir) + Realm.Maps.Pathfinding.GetCellChars(MapCell) + Realm.Maps.Pathfinding.GetDirChar(newDir) +
                    Realm.Maps.Pathfinding.GetCellChars(newCell));

                var startpath = path.GetStartPath;
                var cellpath = path.RemakePath();

                if (!Realm.Maps.Pathfinding.isValidCell(MapCell, cellpath) && !map.RushablesCells.Contains(newCell))
                    return;

                if (cellpath != "")
                {
                    MapCell = path.Destination;
                    Dir = path.Direction;

                    var packet = string.Format("GA0;1;{0};{1}", ID, startpath + cellpath);

                    map.Send(packet);
                }
            }
        }
    }
}
