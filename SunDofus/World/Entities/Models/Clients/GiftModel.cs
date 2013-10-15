using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters.Items;

namespace SunDofus.World.Entities.Models.Clients
{
    class GiftModel
    {
        public int ID { get; set; }
        public int ItemID { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }

        public CharacterItem Item { get; set; }
    }
}
