using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Exchanges
{
    class ExchangePlayer
    {
        private long kamas;

        public bool isNpc;

        public Characters.Character Character;
        public Characters.NPC.NPCMap Npc;

        public List<ExchangeItem> Items;

        public int ID
        {
            get
            {
                if (isNpc)
                    return Npc.ID;
                else
                    return Character.ID;
            }
        }

        public long Kamas
        {
            get
            {
                return kamas;
            }
            set
            {
                kamas = value;
            }
        }

        public ExchangePlayer(Characters.Character _character)
        {
            isNpc = false;
            Character = _character;

            Items = new List<ExchangeItem>();
        }

        public ExchangePlayer(Characters.NPC.NPCMap _npc)
        {
            isNpc = true;
            Npc = _npc;

            Items = new List<ExchangeItem>();
        }

        public void Send(string message)
        {
            if (!isNpc)
                Character.NetworkClient.Send(message);
        }
    }
}
