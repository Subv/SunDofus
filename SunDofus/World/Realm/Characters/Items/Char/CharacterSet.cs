using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Realm.Characters.Items
{
    class CharacterSet
    {
        private int _ID;

        public int ID
        {
            get
            {
                return _ID;
            }
        }
        public List<int> ItemsList;
        public Dictionary<int, List<Effects.EffectItem>> BonusList;

        public CharacterSet(int id)
        {
            _ID = id;

            ItemsList = new List<int>();
            BonusList = Entities.Requests.ItemsRequests.SetsList.First(x => x.ID == ID).BonusList;
            BonusList[1] = new List<Effects.EffectItem>();
        }
    }
}
