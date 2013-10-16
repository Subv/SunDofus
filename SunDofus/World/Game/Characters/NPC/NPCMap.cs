using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using SunDofus.World.Entities.Models.NPC;

namespace SunDofus.World.Game.Characters.NPC
{
    class NPCMap
    {
        public int ID { get; set; }
        public int MapID { get; set; }
        public int MapCell { get; set; }
        public int Dir { get; set; }

        public bool MustMove { get; set; }

        public NoPlayerCharacterModel Model { get; set; }
        private Timer timer;

        public NPCMap(NoPlayerCharacterModel model)
        {
            Model = model;
        }

        public void StartMove()
        {
            if (MustMove == false || !Utilities.Config.GetBoolElement("MUSTNPCSMOVE"))
                return;

            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = Utilities.Basic.Rand(5000, 15000);
            timer.Elapsed += new ElapsedEventHandler(this.Move);
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
            timer.Interval = Utilities.Basic.Rand(5000, 15000);

            var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == MapID);

            var path = new Game.Maps.Pathfinding("", map, MapCell, Dir);
            var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            var newCell = path.NextCell(MapCell, newDir);

            if (newCell <= 0)
                return;

            path.UpdatePath(Game.Maps.Pathfinding.GetDirChar(Dir) + Game.Maps.Pathfinding.GetCellChars(MapCell) + Game.Maps.Pathfinding.GetDirChar(newDir) +
                Game.Maps.Pathfinding.GetCellChars(newCell));

            var startpath = path.GetStartPath;
            var cellpath = path.RemakePath();

            if (!map.RushablesCells.Contains(newCell))
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
