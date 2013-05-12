using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game;
using SunDofus.World.Game.Characters;

namespace SunDofus.World.Entities.Models.Items
{
    class ItemUsableModel
    {
        private int _base;
        private string _args;

        public int Base
        {
            get
            {
                return _base;
            }
            set
            {
                _base = value;
            }
        }
        public string Args
        {
            get
            {
                return _args;
            }
            set
            {
                _args = value;
            }
        }
        public bool MustDelete;

        public ItemUsableModel()
        {
            Base = -1;
            Args = "";
            MustDelete = true;
        }

        public void AttributeItem()
        {
            if (Entities.Requests.ItemsRequests.ItemsList.Any(x => x.ID == Base))
                Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == Base).isUsable = true;
        }

        public void ParseEffect(Character client)
        {
            var datas = Args.Split('|');

            foreach (var effect in datas)
            {
                var infos = effect.Split(';');
                Game.Effects.EffectAction.ParseEffect(client, int.Parse(infos[0]), infos[1]);
            }
        }
    }
}
