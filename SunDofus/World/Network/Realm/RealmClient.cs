using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus.World.Game.Characters;
using SunDofus.World.Entities.Models.Clients;

namespace SunDofus.World.Network.Realm
{
    class RealmClient : Master.TCPClient
    {
        public bool Authentified { get; set; }

        public Character Player { get; set; }
        public List<Character> Characters { get; set; }

        public List<string> Friends { get; set; }
        public List<string> Enemies { get; set; }

        public AccountModel Infos { get; set; }
        public RealmCommander Commander { get; set; }

        private object locker;
        private RealmParser parser;

        public RealmClient(SilverSocket socket) :  base(socket)
        {
            locker = new object();

            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.ReceivedPackets);

            Characters = new List<SunDofus.World.Game.Characters.Character>();
            Friends = new List<string>();
            Enemies = new List<string>();

            Commander = new RealmCommander(this);
            parser = new RealmParser(this);

            Player = null;
            Authentified = false;

            Send("HG");
        }

        public void Send(string message)
        {
            lock(locker)
                this.SendDatas(message);

            Utilities.Loggers.Debug.Write(string.Format("Sent to <{0}> : {1}", IP, message));
        }

        public void ParseCharacters()
        {
            foreach (var name in Infos.Characters)
            {
                if (!SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == name))
                {
                    Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedCharacterPacket().GetPacket(Infos.ID, name));
                    continue;
                }

                var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == name);
                Characters.Add(character);
            }
        }

        public void SendGifts()
        {
            Infos.ParseGifts();

            foreach (var gift in Infos.Gifts)
            {
                if (!Entities.Requests.ItemsRequests.ItemsList.Any(x => x.ID == gift.ItemID))
                    return;

                var item = new SunDofus.World.Game.Characters.Items.CharacterItem(Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == gift.ItemID));

                item.GeneratItem();

                gift.Item = item;

                this.Send(string.Format("Ag1|{0}|{1}|{2}|{3}|{4}~{5}~{6}~~{7};", gift.ID, gift.Title, gift.Message, (gift.Image != "" ? gift.Image : "http://s2.e-monsite.com/2009/12/26/04/167wpr7.png"),
                   Utilities.Basic.DeciToHex(item.Model.ID), Utilities.Basic.DeciToHex(item.Model.ID), Utilities.Basic.DeciToHex(item.Quantity), item.EffectsInfos()));
            }
        }

        public void SendConsoleMessage(string message, int color = 1)
        {
            Send(string.Concat("BAT", color, message));
        }

        public void SendMessage(string message)
        {
            Send(string.Format("cs<font color=\"#00A611\">{0}</font>", message));
        }

        private void ReceivedPackets(string datas)
        {
            Utilities.Loggers.Debug.Write(string.Format("Receive datas from @<{0}>@ : {1}", IP, datas));

            lock (locker)
                parser.Parse(datas);
        }

        private void Disconnected()
        {
            Utilities.Loggers.Debug.Write(string.Format("New closed client @<{0}>@ connection !", IP));

            if (Authentified == true)
            {
                Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.ClientDisconnectedPacket().GetPacket(Infos.Pseudo));

                if (Player != null)
                {
                    Player.GetMap().DelPlayer(Player);
                    Player.IsConnected = false;

                    if (Player.State.OnExchange)
                        SunDofus.World.Game.Exchanges.ExchangesManager.LeaveExchange(Player);

                    if (Player.State.OnWaitingGuild)
                    {
                        if (Player.State.ReceiverInviteGuild != -1 || Player.State.SenderInviteGuild != -1)
                        {
                            if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any
                                (x => x.ID == (Player.State.ReceiverInviteGuild != -1 ? Player.State.ReceiverInviteGuild : Player.State.SenderInviteGuild)))
                            {

                                var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First
                                    (x => x.ID == (Player.State.ReceiverInviteGuild != -1 ? Player.State.ReceiverInviteGuild : Player.State.SenderInviteGuild));
                                if (character.IsConnected)
                                {
                                    character.State.SenderInviteGuild = -1;
                                    character.State.ReceiverInviteGuild = -1;
                                    character.State.OnWaitingGuild = false;
                                    character.NClient.Send("gJEc");
                                }

                                Player.State.ReceiverInviteGuild = -1;
                                Player.State.SenderInviteGuild = -1;
                                Player.State.OnWaitingGuild = false;
                            }
                        }
                    }

                    if (Player.State.OnWaitingParty)
                    {
                        if (Player.State.ReceiverInviteParty != -1 || Player.State.SenderInviteParty != -1)
                        {
                            if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any
                                (x => x.ID == (Player.State.ReceiverInviteParty != -1 ? Player.State.ReceiverInviteParty : Player.State.SenderInviteParty)))
                            {

                                var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First
                                    (x => x.ID == (Player.State.ReceiverInviteParty != -1 ? Player.State.ReceiverInviteParty : Player.State.SenderInviteParty));
                                if (character.IsConnected)
                                {
                                    character.State.SenderInviteParty = -1;
                                    character.State.ReceiverInviteParty = -1;
                                    character.State.OnWaitingParty = false;
                                    character.NClient.Send("PR");
                                }

                                Player.State.ReceiverInviteParty = -1;
                                Player.State.SenderInviteParty = -1;
                                Player.State.OnWaitingParty = false;
                            }
                        }
                    }

                    if (Player.State.Party != null)
                        Player.State.Party.LeaveParty(Player.Name);

                    if (Player.State.IsFollowing)
                    {
                        if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.State.Followers.Contains(Player) && x.ID == Player.State.FollowingID))
                            SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Player.State.FollowingID).State.Followers.Remove(Player);
                    }

                    if (Player.State.IsFollow)
                    {
                        Player.State.Followers.Clear();
                        Player.State.IsFollow = false;
                    }

                    if (Player.State.IsChallengeAsked)
                    {
                        if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.State.ChallengeAsked == Player.ID))
                        {
                            var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.State.ChallengeAsked == Player.ID);

                            Player.State.ChallengeAsker = -1;
                            Player.State.IsChallengeAsked = false;

                            character.State.ChallengeAsked = -1;
                            character.State.IsChallengeAsker = false;

                            character.NClient.Send(string.Format("GA;902;{0};{1}", character.ID, Player.ID));
                        }
                    }

                    if (Player.State.IsChallengeAsker)
                    {
                        if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.State.ChallengeAsker == Player.ID))
                        {
                            var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.State.ChallengeAsker == Player.ID);

                            Player.State.ChallengeAsked = -1;
                            Player.State.IsChallengeAsker = false;

                            character.State.ChallengeAsker = -1;
                            character.State.IsChallengeAsked = false;

                            character.NClient.Send(string.Format("GA;902;{0};{1}", character.ID, Player.ID));
                        }
                    }
                }
            }

            if (Authentified)
            {
                lock (ServersHandler.RealmServer.PseudoClients)
                    ServersHandler.RealmServer.PseudoClients.Remove(Infos.Pseudo);
            }

            lock(Network.ServersHandler.RealmServer.Clients)
                Network.ServersHandler.RealmServer.Clients.Remove(this);
        }
    }
}
