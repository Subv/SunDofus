using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Exchanges
{
    class Exchange
    {
        public Characters.Character player1;

        private long _player1Gold;
        private List<ExchangeItem> _player1Items = new List<ExchangeItem>();

        public Characters.Character player2;

        private long _player2Gold;
        private List<ExchangeItem> _player2Items = new List<ExchangeItem>();

        public Exchange(Characters.Character new1, Characters.Character new2)
        {
            player1 = new1;
            player2 = new2;
        }

        public void Reset()
        {
            player1.State.onExchangeAccepted = false;
            player2.State.onExchangeAccepted = false;

            player1.NetworkClient.Send(string.Format("EK0{0}", player1.ID));
            player1.NetworkClient.Send(string.Format("EK0{0}", player2.ID));
            player2.NetworkClient.Send(string.Format("EK0{0}", player2.ID));
            player2.NetworkClient.Send(string.Format("EK0{0}", player1.ID));
        }

        public void MoveGold(Characters.Character character, long kamas)
        {
            if (player1 == character)
            {
                Reset();
                _player1Gold = kamas;

                player1.NetworkClient.Send(string.Format("EMKG{0}", kamas));
                player2.NetworkClient.Send(string.Format("EmKG{0}", kamas));
            }
            else if (player2 == character)
            {
                Reset();
                _player2Gold = kamas;

                player2.NetworkClient.Send(string.Format("EMKG{0}", kamas));
                player1.NetworkClient.Send(string.Format("EmKG{0}", kamas));
            }
        }

        public void MoveItem(Characters.Character character, Characters.Items.CharacterItem item, int quantity, bool add)
        {
            if (player1 == character)
            {
                if (add)
                {
                    Reset();

                    lock (_player1Items)
                    {
                        if (_player1Items.Any(x => x.Item == item))
                        {
                            var item2 = _player1Items.First(x => x.Item == item);
                            item2.Quantity += quantity;

                            if (item2.Quantity > item.Quantity)
                                item2.Quantity = item.Quantity;

                            player1.NetworkClient.Send(string.Format("EMKO+{0}|{1}", item.ID, item2.Quantity));
                            player2.NetworkClient.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, item2.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                        else
                        {
                            var newItem = new ExchangeItem(item);
                            newItem.Quantity = quantity;

                            _player1Items.Add(newItem);

                            player1.NetworkClient.Send(string.Format("EMKO+{0}|{1}", item.ID, newItem.Quantity));
                            player2.NetworkClient.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, newItem.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                    }
                }
                else
                {
                    Reset();

                    lock (_player1Items)
                    {
                        var Item = _player1Items.First(x => x.Item == item);
                        if (Item.Quantity <= quantity)
                        {
                            player1.NetworkClient.Send(string.Format("EMKO-{0}", Item.Item.ID));
                            player2.NetworkClient.Send(string.Format("EmKO-{0}", Item.Item.ID));
                            _player1Items.Remove(Item);
                        }
                        else
                        {
                            Item.Quantity -= quantity;
                            player1.NetworkClient.Send(string.Format("EMKO+{0}|{1}", Item.Item.ID, Item.Quantity));
                            player2.NetworkClient.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", Item.Item.ID, Item.Quantity, Item.Item.Model.ID, Item.Item.EffectsInfos()));
                        }
                    }
                }
            }
            else if (player2 == character)
            {
                if (add)
                {
                    Reset();

                    lock (_player2Items)
                    {
                        if (_player2Items.Any(x => x.Item == item))
                        {
                            var item2 = _player2Items.First(x => x.Item == item);
                            item2.Quantity += quantity;

                            if (item2.Quantity > item.Quantity)
                                item2.Quantity = item.Quantity;

                            player2.NetworkClient.Send(string.Format("EMKO+{0}|{1}", item.ID, item2.Quantity));
                            player1.NetworkClient.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, item2.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                        else
                        {
                            var newItem = new ExchangeItem(item);
                            newItem.Quantity = quantity;

                            _player2Items.Add(newItem);

                            player2.NetworkClient.Send(string.Format("EMKO+{0}|{1}", item.ID, newItem.Quantity));
                            player1.NetworkClient.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, newItem.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                    }
                }
                else
                {
                    Reset();

                    lock (_player2Items)
                    {
                        var Item = _player2Items.First(x => x.Item == item);
                        if (Item.Quantity <= quantity)
                        {
                            player2.NetworkClient.Send(string.Format("EMKO-{0}", Item.Item.ID));
                            player1.NetworkClient.Send(string.Format("EmKO-{0}", Item.Item.ID));
                            _player2Items.Remove(Item);
                        }
                        else
                        {
                            Item.Quantity -= quantity;
                            player2.NetworkClient.Send(string.Format("EMKO+{0}|{1}", Item.Item.ID, Item.Quantity));
                            player1.NetworkClient.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", Item.Item.ID, Item.Quantity, Item.Item.Model.ID, Item.Item.EffectsInfos()));
                        }
                    }
                }
            }
        }

        public void ValideExchange()
        {
            player1.Kamas = player1.Kamas - _player1Gold + _player2Gold;
            player2.Kamas = player2.Kamas - _player2Gold + _player1Gold;

            foreach (var item in _player1Items)
            {
                var charItem = item.GetNewItem();
                player2.ItemsInventary.AddItem(charItem, false);

                player1.ItemsInventary.DeleteItem(item.Item.ID, item.Quantity);
            }

            foreach (var item in _player2Items)
            {
                var charItem = item.GetNewItem();
                player1.ItemsInventary.AddItem(charItem, false);

                player2.ItemsInventary.DeleteItem(item.Item.ID, item.Quantity);
            }

            player1.SendChararacterStats();
            player2.SendChararacterStats();

            player1.NetworkClient.Send("EVa");
            player2.NetworkClient.Send("EVa");

            ExchangesManager.LeaveExchange(player1, false);
        }
    }
}
