﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Realm;
using SunDofus.World.Realm.Characters;

namespace SunDofus.World.Entities.Models.Items
{
    class ItemUsableModel
    {
        public int Base;
        public string Args;
        public bool MustDelete;

        public ItemUsableModel()
        {
            Base = -1;
            Args = "";
            MustDelete = true;
        }

        public void AttributeItem()
        {
            if (Entities.Cache.ItemsCache.ItemsList.Any(x => x.ID == Base))
                Entities.Cache.ItemsCache.ItemsList.First(x => x.ID == Base).isUsable = true;
        }

        public void ParseEffect(Character client)
        {
            var datas = Args.Split('|');

            foreach (var effect in datas)
            {
                var infos = effect.Split(';');
                Realm.Effects.EffectAction.ParseEffect(client, int.Parse(infos[0]), infos[1]);
            }
        }
    }
}
