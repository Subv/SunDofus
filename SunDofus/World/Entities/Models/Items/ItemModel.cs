using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Items
{
    class ItemModel
    {
        private int _ID, _type, _level, _pods, _price, _set;

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
        public int Type 
        { 
            get 
            { 
                return _type; 
            }
            set 
            {
                _type = value;
            }
        }
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }
        public int Pods
        {
            get
            {
                return _pods;
            }
            set
            {
                _pods = value;
            }
        }
        public int Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }
        public int Set
        {
            get
            {
                return _set;
            }
            set
            {
                _set = value;
            }
        }

        public bool isUsable;
        public bool isTwoHands;

        public string Jet;
        public string Condistr;

        public List<Game.Effects.EffectItem> EffectsList;

        public ItemModel()
        {
            Price = 0;
            Set = -1;
            Jet = "";
            isTwoHands = false;
            isUsable = false;

            EffectsList = new List<Game.Effects.EffectItem>();
        }

        public void ParseWeaponInfos(string datas)
        { }

        public void ParseRandomJet()
        {
            if (EffectsList.Count != 0)
                return;

            var jet = Jet;

            foreach (var _jet in jet.Split(','))
            {
                if (_jet == "") continue;
                var infos = _jet.Split('#');

                var myEffect = new Game.Effects.EffectItem();
                myEffect.ID = Utilities.Basic.HexToDeci(infos[0]);

                if (infos.Length > 1) myEffect.Value = Utilities.Basic.HexToDeci(infos[1]);
                if (infos.Length > 2) myEffect.Value2 = Utilities.Basic.HexToDeci(infos[2]);
                if (infos.Length > 3) myEffect.Value3 = Utilities.Basic.HexToDeci(infos[3]);
                if (infos.Length > 4) myEffect.Effect = infos[4];

                lock(EffectsList)
                    EffectsList.Add(myEffect);
            }
        }

        public string EffectInfos()
        {
            return string.Join(",", EffectsList);
        }
    }
}
