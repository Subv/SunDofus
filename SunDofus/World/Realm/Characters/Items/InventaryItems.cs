﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Realm.Characters.Items
{
    class InventaryItems
    {
        public Character Client;
        public List<CharacterItem> ItemsList;
        public Dictionary<int, CharacterSet> SetsList;

        public InventaryItems(Character character)
        {
            Client = character;

            ItemsList = new List<CharacterItem>();
            SetsList = new Dictionary<int,CharacterSet>();
        }

        public void AddItem(int id, bool offline, int jet = 4)
        {
            if (offline == true)
            {
                if (!Entities.Cache.ItemsCache.ItemsList.Any(x => x.ID == id))
                    return;

                var baseItem = Entities.Cache.ItemsCache.ItemsList.First(x => x.ID == id);
                var item = new CharacterItem(baseItem);

                item.GeneratItem(jet);

                lock (ItemsList)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();

                    ItemsList.Add(item);

                    Client.Pods += item.Model.Pods;
                }
            }
            else if (offline == false)
            {
                if (!Entities.Cache.ItemsCache.ItemsList.Any(x => x.ID == id))
                    return;

                var baseItem = Entities.Cache.ItemsCache.ItemsList.First(x => x.ID == id);
                var item = new CharacterItem(baseItem);

                item.GeneratItem(jet);

                lock (ItemsList)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        RefreshBonus();
                        Client.NetworkClient.Send(string.Format("OQ{0}|{1}", item2.ID, item2.Quantity));

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();

                    ItemsList.Add(item);
                }

                Client.Pods += item.Model.Pods;
                RefreshBonus();

                Client.NetworkClient.Send(string.Format("OAKO{0}", item.ToString()));
            }
        }

        public void AddItem(CharacterItem item, bool offline)
        {
            lock (ItemsList)
            {
                if (offline == true)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();

                    ItemsList.Add(item);
                    Client.Pods += item.Model.Pods;
                }
                else if (offline == false)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        RefreshBonus();
                        Client.NetworkClient.Send(string.Format("OQ{0}|{1}", item2.ID, item2.Quantity));

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();
                    ItemsList.Add(item);

                    Client.Pods += item.Model.Pods;
                    RefreshBonus();

                    Client.NetworkClient.Send(string.Format("OAKO{0}", item.ToString()));
                }
            }
        }

        public void DeleteItem(int id, int quantity)
        {
            lock (ItemsList)
            {
                if (ItemsList.Any(x => x.ID == id))
                {
                    var item = ItemsList.First(x => x.ID == id);

                    if (item.Quantity <= quantity)
                    {
                        Client.Pods -= (item.Quantity * item.Model.Pods);

                        ItemsList.Remove(item);
                        Client.NetworkClient.Send(string.Format("OR{0}", item.ID));

                        RefreshBonus();
                    }
                    else
                    {
                        Client.Pods -= (quantity * item.Model.Pods);

                        item.Quantity -= quantity;
                        Client.NetworkClient.Send(string.Format("OQ{0}|{1}", item.ID, item.Quantity));

                        RefreshBonus();
                    }
                }
                else
                    Client.NetworkClient.Send("BN");
            }
        }

        public void MoveItem(int id, int pos, int quantity)
        {
            if (!ItemsList.Any(x => x.ID == id))
                return;

            var item = ItemsList.First(x => x.ID == id);

            if (ItemsHandler.PositionAvaliable(item.Model.Type, item.Model.isUsable, pos) == false
                || pos == 1 && item.Model.isTwoHands == true && isOccuptedPos(15) || pos == 15 && isOccuptedPos(1))
            {
                Client.NetworkClient.Send("BN");
                return;
            }

            if (!ItemsHandler.ConditionsAvaliable(item.Model, Client))
            {
                Client.NetworkClient.Send("Im119|44");
                return;
            }

            if (item.Model.Type == 23 && pos != -1)
            {
                if (!ItemsList.Any(x => x.Model.ID == item.Model.ID && x.Position != -1 && x.Model.Type == 23))
                {
                    Client.NetworkClient.Send("OAEA");
                    return;
                }
            }

            if (item.Model.Level > Client.Level)
            {
                Client.NetworkClient.Send("OAEL");
                return;
            }

            item.Position = pos;

            if (item.Position == -1)
            {
                if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position &&
                    x.ID != item.ID))
                {
                    var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position &&
                    x.ID != item.ID);

                    item2.Quantity += item.Quantity;
                    Client.Pods += (item.Model.Pods * item.Quantity);
                    RefreshBonus();

                    Client.NetworkClient.Send(string.Format("OQ{0}|{1}", item2.ID, item2.Quantity));
                    DeleteItem(item.ID, item.Quantity);
                    return;
                }
            }
            else
            {
                if (item.Quantity > 1)
                {
                    if (item.Model.Type == 12 || item.Model.Type == 13 || item.Model.Type == 14 || item.Model.Type == 28 ||
                        item.Model.Type == 33 || item.Model.Type == 37 || item.Model.Type == 42 || item.Model.Type == 49 ||
                        item.Model.Type == 69 || item.Model.Type == 87)
                    {
                        if (quantity <= 0)
                            return;

                        var Copy = item.Copy();
                        Copy.Quantity -= quantity;

                        if (item.Quantity == quantity)
                            Copy.Position = pos;
                        else
                            Copy.Position = -1;

                        item.Quantity = quantity;
                        AddItem(Copy, false);
                    }
                    else
                    {
                        var Copy = item.Copy();

                        Copy.Quantity -= 1;
                        Copy.Position = -1;

                        item.Quantity = 1;
                        AddItem(Copy, false);
                    }

                    Client.NetworkClient.Send(string.Format("OQ{0}|{1}", item.ID, item.Quantity));
                }
            }

            Client.NetworkClient.Send(string.Format("OM{0}|{1}", item.ID, (item.Position != -1 ? item.Position.ToString() : "")));
            Client.GetMap().Send(string.Format("Oa{0}|{1}", Client.ID, Client.GetItemsPos()));

            RefreshBonus();
        }

        public bool isOccuptedPos(int pos)
        {
            return ItemsList.Any(x => x.Position == pos);
        }

        public void ParseItems(string datas)
        {
            var splited = datas.Split(';');

            foreach (var infos in splited)
            {
                var allInfos = infos.Split('~');
                var item = new CharacterItem(Entities.Cache.ItemsCache.ItemsList.First(x => x.ID == Convert.ToInt32(allInfos[0], 16)));
                item.EffectsList.Clear();

                item.ID = ItemsHandler.GetNewID();
                item.Quantity = Convert.ToInt32(allInfos[1], 16);

                if (allInfos[2] != "")
                    item.Position = Convert.ToInt32(allInfos[2], 16);
                else
                    item.Position = -1;

                if (allInfos[3] != "")
                {
                    var effectsList = allInfos[3].Split(',');

                    foreach (var effect in effectsList)
                    {
                        var NewEffect = new Effects.EffectItem();
                        string[] EffectInfos = effect.Split('#');

                        NewEffect.ID = Convert.ToInt32(EffectInfos[0], 16);

                        if (EffectInfos[1] != "")
                            NewEffect.Value = Convert.ToInt32(EffectInfos[1], 16);

                        if (EffectInfos[2] != "")
                            NewEffect.Value2 = Convert.ToInt32(EffectInfos[2], 16);

                        if (EffectInfos[3] != "")
                            NewEffect.Value3 = Convert.ToInt32(EffectInfos[3], 16);

                        NewEffect.Effect = EffectInfos[4];

                        lock(item.EffectsList)
                            item.EffectsList.Add(NewEffect);
                    }

                }

                Client.Pods += (item.Model.Pods * item.Quantity);

                lock(ItemsList)
                    ItemsList.Add(item);
            }
        }

        public void RefreshBonus()
        {
            Client.ResetItemsStats();
            SetsList.Clear();

            foreach (var item in ItemsList)
            {
                if (item.Position != -1 && item.Position < 23)
                {
                    foreach (var effect in item.EffectsList)
                        effect.ParseEffect(Client);
                }
                if (item.Model.Set != -1 && item.Position != -1)
                {
                    if (SetsList.ContainsKey(item.Model.Set))
                    {
                        if (!SetsList[item.Model.Set].ItemsList.Contains(item.Model.ID))
                            SetsList[item.Model.Set].ItemsList.Add(item.Model.ID);
                    }
                    else
                    {
                        SetsList.Add(item.Model.Set, new CharacterSet(item.Model.Set));
                        SetsList[item.Model.Set].ItemsList.Clear();
                        SetsList[item.Model.Set].ItemsList.Add(item.Model.ID);
                    }
                }
            }

            foreach (var set in SetsList.Values)
            {
                var numberItems = set.ItemsList.Count;
                var strItems = string.Join(";", set.ItemsList);
                var strEffects = "";

                foreach (var effect in set.BonusList[numberItems])
                {
                    strEffects += string.Format("{0},", effect.SetString());
                    effect.ParseEffect(Client);
                }

                Client.NetworkClient.Send(string.Format("OS+{0}|{1}|{2}", set.ID, strItems,
                    (strEffects == "" ? "" : strEffects.Substring(0, strEffects.Length - 1))));
            }

            Client.SendPods();
            Client.SendChararacterStats();
        }

        public void UseItem(string datas)
        {
            if (Client.State.onMove == true)
            {
                Client.NetworkClient.Send("BN");
                return;
            }

            var allDatas = datas.Split('|');

            var itemID = int.Parse(allDatas[0]);
            var charID = Client.ID;
            var cellID = Client.MapCell;

            if (allDatas.Length > 2)
            {
                charID = int.Parse(allDatas[1]);
                cellID = int.Parse(allDatas[2]);
            }

            if (!ItemsList.Any(x => x.ID == itemID))
            {
                Client.NetworkClient.Send("OUE");
                return;
            }

            var item = ItemsList.First(x => x.ID == itemID);

            if (item.Model.isUsable == false)
            {
                Client.NetworkClient.Send("BN");
                return;
            }

            var usable = Entities.Cache.ItemsCache.UsablesList.First(x => x.Base == item.Model.ID);

            var character = CharactersManager.CharactersList.First(x => x.ID == charID);

            if (!ItemsHandler.ConditionsAvaliable(item.Model, Client))
            {
                Client.NetworkClient.Send("Im119|44");
                return;
            }

            usable.ParseEffect(character);

            if (usable.MustDelete == true)
                DeleteItem(item.ID, 1);
        }
    }
}
