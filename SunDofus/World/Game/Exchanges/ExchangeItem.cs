using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters.Items;

namespace SunDofus.World.Game.Exchanges
{
    class ExchangeItem
    {
        public CharacterItem Item;

        private int _quantity;

        public int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
            }
        }

        public ExchangeItem(CharacterItem item)
        {
            Item = item;
        }

        public CharacterItem GetNewItem()
        {
            var item = new CharacterItem(Item.Model);

            item.EffectsList.Clear();

            lock(Item.EffectsList)
                Item.EffectsList.ForEach(x => item.EffectsList.Add(new Effects.EffectItem(x)));

            item.ID = ItemsHandler.GetNewID();
            item.Position = Item.Position;

            item.Quantity = Quantity;

            return item;
        }
    }
}
