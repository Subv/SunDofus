using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SunDofus.World.Game.Maps.Monsters
{
    class MonstersGroup
    {
        public List<Monster> Monsters;

        private int _id, _maxSize;

        public int ID
        {
            get
            {
                return _id;
            }
        }
        public int MaxSize
        {
            get
            {
                return _maxSize;
            }
        }

        private Map _map;
        private int _cell;
        private int _dir;

        private Timer _movements;
        private Dictionary<int, List<int>> _base;

        public MonstersGroup(Dictionary<int, List<int>> monsters, Map map)
        {

            Monsters = new List<Monster>();
            _base = monsters;

            _map = map;
            _maxSize = map.Model.MaxGroupSize;

            _id = map.NextNpcID();

            RefreshMappos();
            RefreshMonsters();

            if (Utilities.Config.GetBoolElement("MustMonstersMove"))
            {
                _movements = new Timer();
                _movements.Enabled = true;
                _movements.Interval = Utilities.Basic.Rand(10000, 15000);
                _movements.Elapsed += new ElapsedEventHandler(this.Move);
            }
        }

        private void Move(object e, EventArgs e2)
        {
            _movements.Interval = Utilities.Basic.Rand(10000, 15000);

            var path = new Game.Maps.Pathfinding("", _map, _cell, _dir);
            var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            var newCell = path.NextCell(_cell, newDir);

            if (newCell <= 0)
                return;

            path.UpdatePath(Game.Maps.Pathfinding.GetDirChar(_dir) + Game.Maps.Pathfinding.GetCellChars(_cell) + Game.Maps.Pathfinding.GetDirChar(newDir) +
                Game.Maps.Pathfinding.GetCellChars(newCell));

            var startpath = path.GetStartPath;
            var cellpath = path.RemakePath();

            if (!_map.RushablesCells.Contains(newCell))
                return;

            if (cellpath != "")
            {
                _cell = path.Destination;
                _dir = path.Direction;

                var packet = string.Format("GA0;1;{0};{1}", ID, startpath + cellpath);

                _map.Send(packet);
            }
        }

        private void RefreshMonsters()
        {
            var i = Utilities.Basic.Rand(1, MaxSize);
            for (int size = 1; size <= i; size++)
            {
                var mob = ReturnNewMonster();

                if (mob == null)
                    continue;

                lock(Monsters)
                    Monsters.Add(mob);
            }
        }

        private Monster ReturnNewMonster()
        {
            var key = _base.Keys.ToList()[Utilities.Basic.Rand(0, _base.Count - 1)];
            var value = _base[key][Utilities.Basic.Rand(0, _base[key].Count - 1)];

            if (!Entities.Requests.MonstersRequests.MonstersList.Any(x => x.ID == key))
                return null;

            return new Monster(Entities.Requests.MonstersRequests.MonstersList.First(x => x.ID == key), value);
        }

        private void RefreshMappos()
        {
            _dir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            _cell = _map.RushablesCells[Utilities.Basic.Rand(0, _map.RushablesCells.Count - 1)];
        }

        public string PatternOnMap()
        {
            var packet = string.Format("|+{0};{1};0;{2};", _cell, _dir, ID);

            var ids = "";
            var skins = "";
            var lvls = "";
            var colors = "";

            var first = true;
            foreach (var monster in Monsters)
            {
                if (first)
                    first = false;

                else
                {
                    ids += ",";
                    skins += ",";
                    lvls += ",";
                    colors += ",";
                }

                var model = monster.Model;
                ids += model.ID;
                skins += model.GfxID + "^100";
                lvls += monster.Level;

                colors += string.Format("{0},{1},{2};0,0,0,0", Utilities.Basic.DeciToHex(model.Color),
                    Utilities.Basic.DeciToHex(model.Color2), Utilities.Basic.DeciToHex(model.Color3));
            }

            packet += string.Format("{0};-3;{1};{2};{3}", ids, skins, lvls, colors);

            return packet;
        }
    }
}
