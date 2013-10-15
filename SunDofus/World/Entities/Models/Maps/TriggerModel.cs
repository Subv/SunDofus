﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.World.Conditions;

namespace SunDofus.World.Entities.Models.Maps
{
    class TriggerModel
    {
        public int MapID { get; set; }
        public int CellID { get; set; }
        public int ActionID { get; set; }

        public string Args { get; set; }
        public string Conditions { get; set; }
    }
}
