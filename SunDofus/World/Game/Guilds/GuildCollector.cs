using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SunDofus.World.Game.Guilds
{
    class GuildCollector
    {
        private int _ID;
        private int _owner;
        private int _cell;
        private int _dir;

        private int[] _name;

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
        public int Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }
        public int Cell
        {
            get
            {
                return _cell;
            }
            set
            {
                _cell = value;
            }
        }
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

        public int[] Name
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

        public bool isInFight;
        public bool isNewCollector = false;
        public bool mustDelete = false;

        public Guild Guild;
        public Maps.Map Map;
        private Timer _moveTimer;

        public GuildCollector(Maps.Map map, Characters.Character owner, int id)
        {
            ID = id;
            isInFight = false;
            Guild = owner.Guild;

            Map = map;
            Map.Collector = this;

            _owner = owner.ID;
            _cell = owner.MapCell;
            _dir = 3;

            Name = new int[2] { Utilities.Basic.Rand(1, 39), Utilities.Basic.Rand(1, 71) };

            _moveTimer = new Timer();
            _moveTimer.Enabled = true;
            _moveTimer.Interval = Utilities.Basic.Rand(5000, 15000);
            _moveTimer.Elapsed += new ElapsedEventHandler(this.Move);
            _moveTimer.Start();

            Map.Send(string.Format("GM{0}",PatternMap()));
        }

        private void Move(object sender, EventArgs e)
        {
            _moveTimer.Interval = Utilities.Basic.Rand(5000, 15000);

            var path = new Game.Maps.Pathfinding("", Map, Cell, Dir);
            var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            var newCell = path.NextCell(Cell, newDir);

            if (newCell <= 0)
                return;

            path.UpdatePath(Game.Maps.Pathfinding.GetDirChar(Dir) + Game.Maps.Pathfinding.GetCellChars(Cell) + 
                Game.Maps.Pathfinding.GetDirChar(newDir) + Game.Maps.Pathfinding.GetCellChars(newCell));

            var startpath = path.GetStartPath;
            var cellpath = path.RemakePath();

            if (!Map.RushablesCells.Contains(newCell))
                return;

            if (cellpath != "")
            {
                Cell = path.Destination;
                Dir = path.Direction;

                var packet = string.Format("GA0;1;{0};{1}", ID, startpath + cellpath);

                Map.Send(packet);
            }
        }

        public string PatternGuild()
        {
            return string.Concat(ID, ";", Name[0], ",", Name[1], ";", Utilities.Basic.ToBase36(Map.Model.ID),
                ",", Map.Model.PosX, ",", Map.Model.PosY, ";0;0;10000;7;?,?,1,2,3,4,5|");
        }

        public string PatternMap()
        {
            return string.Concat("|+", Cell, ";", Dir, ";0;", ID, ";", Name[0], ",", Name[1],
                ";-6;6000^100;;", Guild.Name, ";", Guild.Emblem);
        }
    }
}
