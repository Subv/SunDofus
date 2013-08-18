using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Entities.Models.Items;

namespace SunDofus.World.Game.Characters.Items
{
    class CharacterItem
    {
        private int _ID, _position, _quantity;

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
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
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

        public ItemModel Model;
        public List<Effects.EffectItem> EffectsList;

        public CharacterItem(ItemModel model)
        {
            Model = model;

            EffectsList = new List<Effects.EffectItem>();

            lock(EffectsList)
                Model.EffectsList.ForEach(x => EffectsList.Add(new Effects.EffectItem(x)));

            _position = -1;
        }

        public string EffectsInfos()
        {
            return string.Join(",", EffectsList);
        }

        public void GeneratItem(int jet = 4)
        {
            this.Quantity = 1;
            this.Position = -1;

            EffectsList.ForEach(x => GetNewJet(x, jet));
        }

        public void GetNewJet(Effects.EffectItem effect, int jet = 3)
        {
            if (effect.ID == 91 | effect.ID == 92 | effect.ID == 93 | effect.ID == 94 | effect.ID == 95 | effect.ID == 96 | effect.ID == 97 | effect.ID == 98 | effect.ID == 99 | effect.ID == 100 | effect.ID == 101) { }
            else if (effect.ID == 800)
            {
                effect.Value3 = 10; // PDV Des familiers !
            }
            else
            {
                effect.Value = Utilities.Basic.GetRandomJet(effect.Effect, jet);
                effect.Value2 = -1;
            }
        }

        public CharacterItem Copy()
        {
            var item = new CharacterItem(Model);
            item.EffectsList.Clear();

            lock(item.EffectsList)
                EffectsList.ForEach(x => item.EffectsList.Add(new Effects.EffectItem(x)));

            item.ID = ItemsHandler.GetNewID();
            item.Position = Position;
            item.Quantity = Quantity;

            return item;
        }

        public string StorageString()
        {
            return string.Format("{0}|{1}|{2}|{3}", ID, Utilities.Basic.DeciToHex(_quantity), Model.ID, EffectsInfos());
        }

        public string SaveString()
        {
            return string.Format("{0}~{1}~{2}~{3}", Utilities.Basic.DeciToHex(Model.ID), Utilities.Basic.DeciToHex(_quantity),
                (_position == -1 ? "" : Utilities.Basic.DeciToHex(_position)), EffectsInfos());
        }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}",Utilities.Basic.DeciToHex(_ID), Utilities.Basic.DeciToHex(Model.ID),
                Utilities.Basic.DeciToHex(_quantity), (_position == -1 ? "" : Utilities.Basic.DeciToHex(_position)), EffectsInfos());
        }
    }
}
