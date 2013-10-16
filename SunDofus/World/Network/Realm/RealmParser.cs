using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters;
using SunDofus.World.Game.Maps;
using SunDofus.World.Game.Characters.Stats;
using SunDofus.World.Game;
using SunDofus.World.Game.World;

namespace SunDofus.World.Network.Realm
{
    class RealmParser
    {
        public RealmClient Client { get; set; }

        private delegate void Packets(string datas);
        private Dictionary<string, Packets> RegisteredPackets;

        public RealmParser(RealmClient client)
        {
            Client = client;

            RegisteredPackets = new Dictionary<string, Packets>();
            RegisterPackets();
        }

        #region Packets

        private void RegisterPackets()
        {
            RegisteredPackets["AA"] = CreateCharacter;
            RegisteredPackets["AB"] = StatsBoosts;
            RegisteredPackets["AD"] = DeleteCharacter;

            RegisteredPackets["Ag"] = SendGifts;
            RegisteredPackets["AG"] = AcceptGift;
            RegisteredPackets["AL"] = SendCharacterList;
            RegisteredPackets["AP"] = SendRandomName;
            RegisteredPackets["AS"] = SelectCharacter;
            RegisteredPackets["AT"] = ParseTicket;
            RegisteredPackets["AV"] = SendCommunauty;
            RegisteredPackets["BA"] = ParseConsoleMessage;
            RegisteredPackets["Ba"] = TeleportByPos;
            RegisteredPackets["BD"] = SendDate;
            RegisteredPackets["BM"] = ParseChatMessage;
            RegisteredPackets["cC"] = ChangeChannel;
            RegisteredPackets["DC"] = DialogCreate;
            RegisteredPackets["DR"] = DialogReply;
            RegisteredPackets["DV"] = DialogExit;
            RegisteredPackets["EA"] = ExchangeAccept;
            RegisteredPackets["EB"] = ExchangeBuy;
            RegisteredPackets["EK"] = ExchangeValidate;
            RegisteredPackets["EM"] = ExchangeMove;
            RegisteredPackets["ER"] = ExchangeRequest;
            RegisteredPackets["ES"] = ExchangeSell;
            RegisteredPackets["EV"] = CancelExchange;
            RegisteredPackets["FA"] = FriendAdd;
            RegisteredPackets["FD"] = FriendDelete;
            RegisteredPackets["FL"] = FriendsList;
            RegisteredPackets["FO"] = FriendsFollow;
            RegisteredPackets["GA"] = GameAction;
            RegisteredPackets["GC"] = CreateGame;
            RegisteredPackets["GI"] = GameInformations;
            RegisteredPackets["GK"] = EndAction;
            RegisteredPackets["GP"] = ChangeAlignmentEnable;
            RegisteredPackets["gB"] = UpgradeStatsGuild;
            RegisteredPackets["gb"] = UpgradeSpellsGuild;
            RegisteredPackets["gC"] = CreateGuild;
            RegisteredPackets["gH"] = LetCollectorGuild;
            RegisteredPackets["gI"] = GetGuildInfos;
            RegisteredPackets["gJ"] = GetGuildJoinRequest;
            RegisteredPackets["gK"] = ExitGuild;
            RegisteredPackets["gP"] = ModifyRightGuild;
            RegisteredPackets["gV"] = CloseGuildPanel;
            RegisteredPackets["iA"] = EnemyAdd;
            RegisteredPackets["iD"] = EnemyDelete;
            RegisteredPackets["iL"] = EnemiesList;
            RegisteredPackets["Od"] = DeleteItem;
            RegisteredPackets["OM"] = MoveItem;
            RegisteredPackets["OU"] = UseItem;
            RegisteredPackets["PA"] = PartyAccept;
            RegisteredPackets["PG"] = PartyGroupFollow;
            RegisteredPackets["PF"] = PartyFollow;
            RegisteredPackets["PI"] = PartyInvite;
            RegisteredPackets["PR"] = PartyRefuse;
            RegisteredPackets["PV"] = PartyLeave;
            RegisteredPackets["SB"] = SpellBoost;
            RegisteredPackets["SM"] = SpellMove;
            RegisteredPackets["WU"] = UseZaaps;
            RegisteredPackets["Wu"] = UseZaapis;
            RegisteredPackets["WV"] = ExitZaap;
            RegisteredPackets["Wv"] = ExitZaapis;
        }

        public void Parse(string datas)
        {
            if (datas == "ping")
                Client.Send("pong");
            else if (datas == "qping")
                Client.Send("qpong");

            if (datas.Length < 2) 
                return;

            string header = datas.Substring(0, 2);

            if (!RegisteredPackets.ContainsKey(header))
            {
                Client.Send("BN");
                return;
            }

            RegisteredPackets[header](datas.Substring(2));
        }

        #endregion

        #region Ticket & Queue

        private void ParseTicket(string datas)
        {
            lock (Network.Auth.AuthKeys.Keys)
            {
                if (Network.Auth.AuthKeys.Keys.Any(x => x.Key == datas))
                {
                    var key = Network.Auth.AuthKeys.Keys.First(x => x.Key == datas);

                    if (ServersHandler.RealmServer.Clients.Any(x => x.Authentified == true && x.Infos.Pseudo == key.Infos.Pseudo))
                        ServersHandler.RealmServer.Clients.First(x => x.Authentified == true && x.Infos.Pseudo == key.Infos.Pseudo).Disconnect();

                    Client.Infos = key.Infos;
                    Client.Infos.ParseCharacters();
                    Client.ParseCharacters();

                    Client.Authentified = true;

                    foreach (var friend in Client.Infos.StrFriends.Split('+'))
                    {
                        if (!Client.Friends.Contains(friend))
                            Client.Friends.Add(friend);
                    }

                    foreach (var enemy in Client.Infos.StrEnemies.Split('+'))
                    {
                        if (!Client.Enemies.Contains(enemy))
                            Client.Enemies.Add(enemy);
                    }

                    Network.Auth.AuthKeys.Keys.Remove(key);

                    Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.ClientConnectedPacket().GetPacket(Client.Infos.Pseudo));

                    lock (ServersHandler.RealmServer.PseudoClients)
                    {
                        if (!ServersHandler.RealmServer.PseudoClients.ContainsKey(Client.Infos.Pseudo))
                            ServersHandler.RealmServer.PseudoClients.Add(Client.Infos.Pseudo, Client.Infos.ID);
                    }

                    Client.Send("ATK0");

                    if ((Environment.TickCount - RealmQueue.LastAction) < Utilities.Config.GetIntElement("TIME_BETWEEN_TWOCONNECTIONS"))
                    {
                        RealmQueue.AddInQueue(Client);
                        RefreshQueue("");
                    }

                    RealmQueue.LastAction = Environment.TickCount;
                }
                else
                    Client.Send("ATE");
            }
        }

        private void RefreshQueue(string datas)
        {
            if (Client.IsInQueue)
            {
                Client.Send(string.Format("Af{0}|{1}|0|2", (RealmQueue.Clients.IndexOf(Client) + 2),
                    (RealmQueue.Clients.Count > 2 ? RealmQueue.Clients.Count : 3)));
            }
        }

        #endregion
        
        #region Character

        private void SendRandomName(string datas)
        {
            Client.Send(string.Concat("APK", Utilities.Basic.RandomName()));
        }

        private void SendCommunauty(string datas)
        {
            Client.Send(string.Concat("AV", Utilities.Config.GetIntElement("SERVERCOM")));
        }

        public void SendCharacterList(string datas)
        {
            if (!Client.IsInQueue)
            {
                string packet = string.Format("ALK{0}|{1}", Client.Infos.Subscription, Client.Characters.Count);

                if (Client.Characters.Count != 0)
                {
                    foreach (SunDofus.World.Game.Characters.Character m_C in Client.Characters)
                        packet += string.Concat("|", m_C.PatternList());
                }

                Client.Send(packet);
            }
        }

        private void CreateCharacter(string datas)
        {
            try
            {
                var characterDatas = datas.Split('|');

                if (characterDatas[0] != "" | World.Entities.Requests.CharactersRequests.ExistsName(characterDatas[0]) == false)
                {
                    var character = new Character();

                    if (World.Entities.Requests.CharactersRequests.CharactersList.Count > 0)
                        character.ID = (World.Entities.Requests.CharactersRequests.CharactersList.OrderByDescending(x => x.ID).ToArray()[0].ID) + 1;
                    else
                        character.ID = 1;

                    character.Name = characterDatas[0];
                    character.Level = Utilities.Config.GetIntElement("STARTLEVEL");
                    character.Class = int.Parse(characterDatas[1]);
                    character.Sex = int.Parse(characterDatas[2]);
                    character.Skin = int.Parse(character.Class + "" + character.Sex);
                    character.Size = 100;
                    character.Color = int.Parse(characterDatas[3]);
                    character.Color2 = int.Parse(characterDatas[4]);
                    character.Color3 = int.Parse(characterDatas[5]);

                    switch (character.Class)
                    {
                        case 1:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_FECA");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_FECA");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_FECA");
                            break;
                        case 2:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_OSA");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_OSA");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_OSA");
                            break;
                        case 3:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_ENU");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_ENU");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_ENU");
                            break;
                        case 4:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_SRAM");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_SRAM");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_SRAM");
                            break;
                        case 5:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_XEL");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_XEL");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_XEL");
                            break;
                        case 6:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_ECA");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_ECA");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_ECA");
                            break;
                        case 7:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_ENI");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_ENI");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_ENI");
                            break;
                        case 8:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_IOP");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_IOP");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_IOP");
                            break;
                        case 9:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_CRA");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_CRA");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_CRA");
                            break;
                        case 10:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_SADI");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_SADI");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_SADI");
                            break;
                        case 11:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_SACRI");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_SACRI");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_SACRI");
                            break;
                        case 12:
                            character.MapID = Utilities.Config.GetIntElement("STARTMAP_PANDA");
                            character.MapCell = Utilities.Config.GetIntElement("STARTCELL_PANDA");
                            character.Dir = Utilities.Config.GetIntElement("STARTDIR_PANDA");
                            break;
                    }

                    character.CharactPoint = (character.Level - 1) * 5;
                    character.SpellPoint = (character.Level - 1);
                    character.Exp = Entities.Requests.LevelsRequests.ReturnLevel(character.Level).Character;
                    character.Kamas = (long)Utilities.Config.GetIntElement("STARTKAMAS");

                    if (character.Class < 1 | character.Class > 12 | character.Sex < 0 | character.Sex > 1)
                    {
                        Client.Send("AAE");
                        return;
                    }

                    character.SpellsInventary.LearnSpells();
                    character.IsNewCharacter = true;

                    lock (World.Entities.Requests.CharactersRequests.CharactersList)
                        World.Entities.Requests.CharactersRequests.CharactersList.Add(character);

                    lock(Client.Characters)
                        Client.Characters.Add(character);

                    Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.CreatedCharacterPacket().GetPacket(Client.Infos.ID, character.Name));

                    Client.Send("AAK");
                    Client.Send("TB");
                    SendCharacterList("");
                }
                else
                {
                    Client.Send("AAE");
                }
            }
            catch (Exception e)
            {
                Utilities.Loggers.Errors.Write(e.ToString());
            }
        }

        private void DeleteCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[0], out id))
                return;

            lock (World.Entities.Requests.CharactersRequests.CharactersList)
            {
                if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == id))
                    return;

                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == id);

                if (datas.Split('|')[1] != Client.Infos.Answer && character.Level >= 20)
                {
                    Client.Send("ADE");
                    return;
                }

                lock(Client.Characters)
                    Client.Characters.Remove(character);

                Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedCharacterPacket().GetPacket(Client.Infos.ID, character.Name));
                character.IsDeletedCharacter = true;

                SendCharacterList("");
            }
        }

        private void SelectCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            lock (World.Entities.Requests.CharactersRequests.CharactersList)
            {
                if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == id))
                    return;

                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == id);

                lock (Client.Characters)
                {
                    if (Client.Characters.Contains(character))
                    {
                        Client.Player = character;
                        Client.Player.State = new CharacterState(Client.Player);
                        Client.Player.NClient = Client;

                        Client.Player.IsConnected = true;

                        foreach (var client in Network.ServersHandler.RealmServer.Clients.Where(x => x.Characters.Any(c => c.IsConnected == true) && x.Friends.Contains(Client.Infos.Pseudo) && x.Player.Friends.WillNotifyWhenConnected))
                            client.Send(string.Concat("Im0143;", Client.Player.Name));

                        Client.Send(string.Concat("ASK", Client.Player.PatternSelect()));
                    }
                    else
                        Client.Send("ASE");
                }
            }
        }

        #endregion

        #region Gift

        private void SendGifts(string datas)
        {
            Client.SendGifts();
        }

        private void AcceptGift(string datas)
        {
            var infos = datas.Split('|');

            var idGift = 0;
            var idChar = 0;

            if (!int.TryParse(infos[0], out idGift) || !int.TryParse(infos[1], out idChar))
                return;

            if (Client.Characters.Any(x => x.ID == idChar))
            {
                lock (Client.Infos.Gifts)
                {
                    if (Client.Infos.Gifts.Any(x => x.ID == idGift))
                    {
                        var myGift = Client.Infos.Gifts.First(e => e.ID == idGift);
                        Client.Characters.First(x => x.ID == idChar).ItemsInventary.AddItem(myGift.Item, true);

                        Client.Send("AG0");
                        Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedGiftPacket().GetPacket(myGift.ID, Client.Infos.ID));

                        lock(Client.Infos.Gifts)
                            Client.Infos.Gifts.Remove(myGift);

                    }
                    else
                        Client.Send("AGE");
                }
            }
            else
                Client.Send("AGE");
        }

        #endregion

        #region World

        #region Date

        private void SendDate(string datas)
        {
            Client.Send(string.Concat("BD", Utilities.Basic.GetDofusDate()));
        }

        #endregion

        #region Channels

        private void ChangeChannel(string channel)
        {
            char head;

            if (!char.TryParse(channel.Substring(1), out head))
                return;
            
            var state = (channel.Substring(0, 1) == "+" ? true : false);
            Client.Player.Channels.ChangeChannelState(head, state);
        }

        #endregion

        #region Faction

        private void ChangeAlignmentEnable(string enable)
        {
            if (enable == "+")
            {
                if (Client.Player.Faction.ID != 0)
                    Client.Player.Faction.IsEnabled = true;
            }
            else if (enable == "*")
            {
                var hloose = Client.Player.Faction.Honor / 100;
                Client.Send(string.Concat("GIP", hloose.ToString()));

                return;
            }
            else if (enable == "-")
            {
                var hloose = Client.Player.Faction.Honor / 100;

                if (Client.Player.Faction.ID != 0)
                {
                    Client.Player.Faction.IsEnabled = false;
                    Client.Player.Faction.Honor -= hloose;
                }
            }

            Client.Player.SendChararacterStats();
        }

        #endregion

        #region Friends - Enemies

        private void FriendsList(string datas)
        {
            Client.Player.Friends.SendFriends();
        }

        private void FriendAdd(string datas)
        {
            Client.Player.Friends.AddFriend(datas);
        }

        private void FriendDelete(string datas)
        {
            Client.Player.Friends.RemoveFriend(datas);
        }

        private void FriendsFollow(string datas)
        {
            if (datas.Substring(0, 1) == "+")
                Client.Player.Friends.WillNotifyWhenConnected = true;
            else
                Client.Player.Friends.WillNotifyWhenConnected = false;

            Client.Send(string.Concat("FO", (Client.Player.Friends.WillNotifyWhenConnected ? "+" : "-")));
        }

        private void EnemiesList(string datas)
        {
            Client.Player.Enemies.SendEnemies();
        }

        private void EnemyAdd(string datas)
        {
            Client.Player.Enemies.AddEnemy(datas);
        }

        private void EnemyDelete(string datas)
        {
            Client.Player.Enemies.RemoveEnemy(datas);
        }

        #endregion

        #region Zaaps - Zaapis

        private void ExitZaap(string datas)
        {
            Client.Send("WV");
        }

        private void ExitZaapis(string datas)
        {
            Client.Send("Wv");
        }

        private void UseZaapis(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            Game.Maps.Zaapis.ZaapisManager.OnMove(Client.Player, id);
        }

        private void UseZaaps(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            Game.Maps.Zaaps.ZaapsManager.OnMove(Client.Player, id);
        }

        #endregion

        #region Chat

        private void ParseChatMessage(string datas)
        {
            var infos = datas.Split('|');

            var channel = infos[0];
            var message = infos[1];

            switch (channel)
            {
                case "*":
                    Chat.SendGeneralMessage(Client, message);
                    return;

                case "^":
                    Chat.SendIncarnamMessage(Client, message);
                    return;

                case "$":
                    Chat.SendPartyMessage(Client, message);
                    return;

                case "%":
                    Chat.SendGuildMessage(Client, message);
                    return;

                case "#":
                    //TeamMessage
                    return;

                case "?":
                    Chat.SendRecruitmentMessage(Client, message);
                    return;

                case "!":
                    Chat.SendFactionMessage(Client, message);
                    return;

                case ":":
                    Chat.SendTradeMessage(Client, message);
                    return;

                case "@":
                    Chat.SendAdminMessage(Client, message);
                    return;

                case "¤":
                    //No idea
                    return;

                default:
                    if (channel.Length > 1)
                        Chat.SendPrivateMessage(Client, channel, message);
                    return;
            }
        }

        private void ParseConsoleMessage(string datas)
        {
            Client.Commander.ParseAdminCommand(datas);
        }

        #endregion

        #region Guilds

        private void CloseGuildPanel(string datas)
        {
            Client.Send(string.Concat("gV", Client.Player.Name));
        }

        private void CreateGuild(string datas)
        {
            //TODO VERIF IF HAST THE CLIENT A GILDAOGEME

            if (Client.Player.Guild != null)
            {
                Client.Player.NClient.Send("Ea");
                return;
            }

            var infos = datas.Split('|');

            if (infos.Length < 5)
            {
                Client.Send("BN");
                return;
            }

            if (infos[0].Contains("-"))
                infos[0] = "1";
            if (infos[1].Contains("-"))
                infos[1] = "0";
            if (infos[2].Contains("-"))
                infos[2] = "1";
            if (infos[3].Contains("-"))
                infos[3] = "0";

            var bgID = 0;
            var bgColor = 0;
            var embID = 0;
            var embColor = 0;

            if (!int.TryParse(infos[0], out bgID) || !int.TryParse(infos[1], out bgColor) ||
                !int.TryParse(infos[2], out embID) || !int.TryParse(infos[3], out embColor))
            {
                Client.Send("BN");
                return;
            }

            if (infos[4].Length > 15 || Entities.Requests.GuildsRequest.GuildsList.Any(x => x.Name == infos[4]))
            {
                Client.Player.NClient.Send("Ean");
                return;
            }

            var ID = (Entities.Requests.GuildsRequest.GuildsList.Count < 1 ? 1 : Entities.Requests.GuildsRequest.GuildsList.OrderByDescending(x => x.ID).ToArray()[0].ID + 1);

            var guild = new World.Game.Guilds.Guild()
            {
                ID = ID,
                Name = infos[4],
                BgID = bgID,
                BgColor = bgColor,
                EmbID = embID,
                EmbColor = embColor,
                Exp = 0,
                Level = 1,
                CollectorMax = 1,
                CollectorProspection = 0,
                CollectorWisdom = 0,
                CollectorPods = 0,
                IsNewGuild = true
            };

            guild.AddMember(new Game.Guilds.GuildMember(Client.Player));

            guild.Spells.Add(462, 1);
            guild.Spells.Add(461, 1);
            guild.Spells.Add(460, 1);
            guild.Spells.Add(459, 1);
            guild.Spells.Add(458, 1);
            guild.Spells.Add(457, 1);
            guild.Spells.Add(456, 1);
            guild.Spells.Add(455, 1);
            guild.Spells.Add(454, 1);
            guild.Spells.Add(453, 1);
            guild.Spells.Add(452, 1);
            guild.Spells.Add(451, 1);

            Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(guild.Members[0].Rights)));
            Client.Send("gV");

            Entities.Requests.GuildsRequest.GuildsList.Add(guild);

            //REMOVE GILDAOGEME
        }

        private void ExitGuild(string datas)
        {
            if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas))
            {
                Client.Send("BN");
                return;
            }            

            //TODO, VERIF DROITS

            if (datas == Client.Player.Name)
            {
                if (Client.Player.Guild == null)
                {
                    Client.Send("BN");
                    return;
                }

                var guild = Client.Player.Guild;

                if (guild.Members.Count < 2)
                {
                    Client.Player.NClient.Send("Im1101");
                    return;
                }

                var member = guild.Members.First(x => x.Character == Client.Player);

                if (member.Rank == 1)
                {
                    Client.Player.NClient.Send("Im1101");
                    return;
                }

                Client.Send(string.Format("gKK{0}|{1}", Client.Player.Name, Client.Player.Name));

                member.Rank = 0;
                member.Rights = 0;
                member.ExpGived = 0;
                member.ExpGaved = 0;

                guild.Members.Remove(member);
                Client.Player.Guild = null;
                Client.Player.NClient.Send("Im0176");
            }
            else
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas);

                if (character.Guild == null || Client.Player.Guild == null || (Client.Player.Guild != character.Guild))
                {
                    Client.Send("BN");
                    return;
                }

                var guild = Client.Player.Guild;

                var member = guild.Members.First(x => x.Character == character);

                if (member.Rank == 1)
                {
                    Client.Player.NClient.Send("Im1101");
                    return;
                }
                
                if(character.IsConnected)
                    character.NClient.Send(string.Format("gKK{0}|{1}", Client.Player.Name, Client.Player.Name));

                member.Rank = 0;
                member.Rights = 0;
                member.ExpGived = 0;
                member.ExpGaved = 0;

                Client.Player.NClient.Send(string.Concat("Im0177;", character.Name));

                guild.Members.Remove(member);
                character.Guild = null;
            }
        }

        private void ModifyRightGuild(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            //TODO VERIF DROITS

            var guild = Client.Player.Guild;
            var memember = guild.Members.First(x => x.Character == Client.Player);
            var infos = datas.Split('|');

            var ID = 0;
            var rank = 0;
            var rights = 0;
            var expgived = 0;

            if (!int.TryParse(infos[0], out ID) || !int.TryParse(infos[1], out rank) || !int.TryParse(infos[3], out rights) ||
                !int.TryParse(infos[2], out expgived) || !World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == ID))
            {
                Client.Send("BN");
                return;
            }

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == ID);

            if(character.Name == Client.Player.Name && Client.Player.Guild.Members.First
                (x => x.Character.Name == Client.Player.Name).Rights != rights)
            {
                Client.Send("BN");
                return;
            }

            var member = guild.Members.First(x => x.Character == character);

            if (member.Rank == 1 && (member.Rights != rights || member.Rank != rank))
            {
                Client.Player.NClient.Send("Im1101");
                return;
            }

            if (expgived > 90)
                expgived = 90;
            else if (expgived < 0)
                expgived = 0;

            if (rank == 1 && member.Rank != 1 && memember.Rank == 1)
            {
                memember.Rank = member.Rank;
                memember.Rights = member.Rights;


                Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(memember.Rights)));

                member.Rank = 1;
                member.Rights = 29695;
                member.ExpGived = expgived;

                if (member.Character.IsConnected)
                    member.Character.NClient.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));

                return;
            }

            member.ExpGived = expgived;
            member.Rights = rights;
            member.Rank = rank;

            if (member.Character.IsConnected)
                member.Character.NClient.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));
        }

        private void UpgradeStatsGuild(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            //TODO, VERIF DROITS

            var guild = Client.Player.Guild;

            switch (datas[0])
            {
                case 'p':

                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorProspection += 1;
                        return;
                    }
                    break;

                case 'x':
                    
                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorWisdom += 1;
                        return;
                    }
                    break;

                case 'o':
                    
                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorPods += 20;
                        return;
                    }
                    break;

                case 'k':

                    if (guild.BoostPoints > 19)
                    {
                        guild.BoostPoints -= 20;
                        guild.CollectorMax += 1;
                        return;
                    }
                    break;
            }

            Client.Send("BN");
        }

        private void UpgradeSpellsGuild(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            var guild = Client.Player.Guild;

            //TODO VERIF DROITS

            var spellID = 0;

            if(!int.TryParse(datas, out spellID) || !guild.Spells.ContainsKey(spellID) || guild.BoostPoints < 5)
            {
                Client.Send("BN");
                return;
            }

            guild.Spells[spellID]++;
            guild.BoostPoints -= 5;

            GetGuildInfos("B");
        }

        private void LetCollectorGuild(string datas)
        {
            var map = Client.Player.GetMap();

            if (Client.Player.Guild == null || map == null)
            {
                Client.Send("BN");
                return;
            }

            var guild = Client.Player.Guild;

            if (guild.CollectorMax <= guild.Collectors.Count)
            {
                Client.Player.NClient.SendMessage("Vous avez trop de percepteurs !");
                return;
            }

            if (map.Collector != null)
            {
                Client.Player.NClient.SendMessage("Un percepteur est déjà présent sur la map !");
                return;
            }

            var ID = Client.Player.GetMap().NextNpcID();

            var collector = new Game.Guilds.GuildCollector(map, Client.Player, ID)
            {
                IsNewCollector = true
            };

            guild.Collectors.Add(collector);
            Entities.Requests.CollectorsRequests.CollectorsList.Add(collector);

            Client.Player.Guild.SendMessage(string.Format("Un percepteur vient d'être posé par <b>{0}</b> en [{1},{2}] !", Client.Player.Name, Client.Player.GetMap().Model.PosX, Client.Player.GetMap().Model.PosY));
            GetGuildInfos("B");
        }

        private void GetGuildInfos(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            var packet = string.Empty;
            var guild = Client.Player.Guild;

            switch (datas[0])
            {
                case 'B':

                    packet = string.Format("gIB{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", guild.CollectorMax, guild.Collectors.Count, (guild.Level * 100), guild.Level,
                          guild.CollectorPods, guild.CollectorProspection, guild.CollectorWisdom, guild.CollectorMax, guild.BoostPoints, (1000 + (10 * guild.Level)), guild.GetSpells());

                    Client.Send(packet);
                    return;

                case 'G':

                    var lastLevel = Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Guild).Where(x => x.Guild <= guild.Exp).ToArray()[0].Guild;
                    var nextLevel = Entities.Requests.LevelsRequests.LevelsList.OrderBy(x => x.Guild).Where(x => x.Guild > guild.Exp).ToArray()[0].Guild;

                    packet = string.Format("gIG1|{0}|{1}|{2}|{3}", guild.Level, lastLevel, guild.Exp, nextLevel);

                    Client.Send(packet);
                    return;

                case 'M':

                    Client.Send(string.Concat("gIM+", string.Join("|", from c in guild.Members select c.Character.PatternGuild())));
                    return;

                case 'T':

                    packet = string.Concat("gITM+", string.Join("|", from c in guild.Collectors select c.PatternGuild()));
                    Client.Send(packet.Substring(0, packet.Length - 1));
                    return;
            }
        }

        private void GetGuildJoinRequest(string datas)
        {
            switch (datas[0])
            {
                case 'R':

                    if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas.Substring(1)))
                    {
                        Client.Send("BN");
                        return;
                    }

                    var receiverCharacter = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas.Substring(1));

                    if (receiverCharacter.Guild != null || Client.Player.Guild == null || !receiverCharacter.IsConnected)
                    {
                        if (receiverCharacter.Guild != null)
                        {
                            Client.Player.NClient.Send("Le joueur est déjà membre d'une guilde !");
                            return;
                        }

                        Client.Send("BN");
                        return;
                    }

                    Client.Player.State.ReceiverInviteGuild = receiverCharacter.ID;
                    receiverCharacter.State.SenderInviteGuild = Client.Player.ID;

                    Client.Player.State.OnWaitingGuild = true;
                    receiverCharacter.State.OnWaitingGuild = true;

                    Client.Send(string.Concat("gJR", receiverCharacter.Name));
                    receiverCharacter.NClient.Send(string.Format("gJr{0}|{1}|{2}", Client.Player.ID, Client.Player.Name, Client.Player.Guild.Name));

                    break;

                case 'K':

                    var ID = 0;

                    if (!int.TryParse(datas.Substring(1), out ID) || !World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == ID))
                    {
                        Client.Send("BN");
                        return;
                    }

                    var accepttoCharacter = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == ID);

                    if (!accepttoCharacter.IsConnected || accepttoCharacter.State.ReceiverInviteGuild != Client.Player.ID)
                    {
                        Client.Send("BN");
                        return;
                    }

                    Client.Player.State.SenderInviteGuild = -1;
                    accepttoCharacter.State.ReceiverInviteGuild = -1;

                    Client.Player.State.OnWaitingGuild = false;
                    accepttoCharacter.State.OnWaitingGuild = false;
                    
                    Client.Player.Guild = accepttoCharacter.Guild;
                    var member = new Game.Guilds.GuildMember(Client.Player);
                    var guild = Client.Player.Guild;

                    accepttoCharacter.Guild.Members.Add(member);

                    Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));
                    accepttoCharacter.NClient.Send(string.Concat("gJKa", Client.Player.Name));

                    break;

                case 'E':

                    if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas.Substring(1)))
                    {
                        Client.Send("BN");
                        return;
                    }

                    var refusetoCharacter = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas.Substring(1));

                    if (!refusetoCharacter.IsConnected || refusetoCharacter.State.ReceiverInviteGuild == Client.Player.ID)
                    {
                        Client.Send("BN");
                        return;
                    }

                    refusetoCharacter.NClient.Send("gJEc");

                    break;
            }
        }

        #endregion

        #region Game

        private void CreateGame(string datas)
        {
            if (Client.Player.SaveMap == 0)
            {
                Client.Player.SaveMap = Client.Player.MapID;
                Client.Player.SaveCell = Client.Player.MapCell;

                Client.Send("Im06");
            }

            Client.Send(string.Concat("GCK|1|", Client.Player.Name));
            Client.Send("AR6bk");

            Client.Player.Channels.SendChannels();
            Client.Send("SLo+");
            Client.Player.SpellsInventary.SendAllSpells();
            Client.Send(string.Concat("BT", Utilities.Basic.GetActuelTime()));

            if (Client.Player.Life == 0)
            {
                Client.Player.UpdateStats();
                Client.Player.Life = Client.Player.MaximumLife;
            }

            Client.Player.ItemsInventary.RefreshBonus();
            Client.Player.SendPods();
            Client.Player.SendChararacterStats();

            Client.Player.LoadMap();

            Client.Send(string.Concat("FO", (Client.Player.Friends.WillNotifyWhenConnected ? "+" : "-")));

            if (Client.Player.Guild != null)
            {
                var guild = Client.Player.Guild;
                Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(guild.Members.First(x => x.Character == Client.Player).Rights)));
            }
        }

        private void TeleportByPos(string packet)
        {
            if (packet[0] != 'M') return;
            if (!packet.Contains(',')) return;

            if (Client.Infos.Level <= Utilities.Config.GetIntElement("MINGMLEVEL_TOTELEPORTWITH_GEOPOSITION"))
                return;

            var pos = packet.Substring(1).Split(',');
            var maps = Entities.Requests.MapsRequests.MapsList.Where(x => x.Model.PosX == int.Parse(pos[0]) && x.Model.PosY == int.Parse(pos[1])).ToArray();

            if (maps.Length > 0)
            {
                Client.Player.TeleportNewMap(maps[0].Model.ID, Client.Player.MapCell);
                Client.SendConsoleMessage("Character Teleported !", 0);
            }
        }

        private void GameInformations(string datas)
        {
            Client.Player.GetMap().AddPlayer(Client.Player);
            Client.Send("GDK");
        }

        private void GameAction(string datas)
        {
            var packet = 0;

            if (!int.TryParse(datas.Substring(0,3), out packet))
                return;

            switch (packet)
            {
                case 1://GameMove
                    GameMove(datas.Substring(3));
                    return;

                case 500://ParseGameAction
                    ParseGameAction(datas.Substring(3));
                    return;

                case 900://AskChallenge
                    //AskChallenge(datas);
                    return;

                case 901://AcceptChallenge
                    //AcceptChallenge(datas);
                    return;

                case 902://RefuseChallenge
                    //RefuseChallenge(datas);
                    return;
            }
        }

        private void ParseGameAction(string packet)
        {
            var infos = packet.Split(';'); 
            
            var id = 0;

            if (!int.TryParse(infos[1], out id))
                return;

            switch (id)
            {
                case 44:
                    Game.Maps.Zaaps.ZaapsManager.SaveZaap(Client.Player);
                    return;

                case 114:
                    Game.Maps.Zaaps.ZaapsManager.SendZaaps(Client.Player);
                    return;

                case 157:
                    Game.Maps.Zaapis.ZaapisManager.SendZaapis(Client.Player);
                    return;

                default:
                    Client.Send("BN");
                    return;
            }
        }

        private void GameMove(string packet)
        {
            var path = new Pathfinding(packet, Client.Player.GetMap(), Client.Player.MapCell, Client.Player.Dir);
            var newPath = path.RemakePath();

            newPath = path.GetStartPath + newPath;

            if (!Client.Player.GetMap().RushablesCells.Contains(path.Destination))
            {
                Client.Send("GA;0");
                return;
            }

            Client.Player.Dir = path.Direction;
            Client.Player.State.MoveToCell = path.Destination;
            Client.Player.State.OnMove = true;

            Client.Player.GetMap().Send(string.Format("GA0;1;{0};{1}", Client.Player.ID, newPath));
        }

        private void EndAction(string datas)
        {
            switch (datas[0])
            {
                case 'K':

                    if (Client.Player.State.OnMove == true)
                    {
                        Client.Player.State.OnMove = false;
                        Client.Player.MapCell = Client.Player.State.MoveToCell;
                        Client.Player.State.MoveToCell = -1;
                        Client.Send("BN");

                        if (Client.Player.GetMap().Triggers.Any(x => x.CellID == Client.Player.MapCell))
                        {
                            var trigger = Client.Player.GetMap().Triggers.First(x => x.CellID == Client.Player.MapCell);

                            if (SunDofus.World.Game.World.Conditions.TriggerCondition.HasConditions(Client.Player, trigger.Conditions))
                                SunDofus.World.Game.Effects.EffectAction.ParseEffect(Client.Player, trigger.ActionID, trigger.Args);
                            else
                                Client.SendMessage("Vous ne possédez pas les conditions nécessaires pour cette action !");
                        }
                    }

                    return;

                case 'E':

                    var cell = 0;

                    if (!int.TryParse(datas.Split('|')[1], out cell))
                        return;

                    Client.Player.State.OnMove = false;
                    Client.Player.MapCell = cell;

                    return;
            }
        }

        #endregion

        #region Challenge

        private void AskChallenge(string datas)
        {
            var charid = 0;

            if(!int.TryParse(datas.Substring(3), out charid))
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid))
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                if (Client.Player.State.Busy || character.State.Busy || Client.Player.GetMap().Model.ID != character.GetMap().Model.ID)
                {
                    Client.SendMessage("Personnage actuellement occupé ou indisponible !");
                    return;
                }

                Client.Player.State.ChallengeAsked = character.ID;
                Client.Player.State.IsChallengeAsker = true;

                character.State.ChallengeAsker = Client.Player.ID;
                character.State.IsChallengeAsked = true;

                Client.Player.GetMap().Send(string.Format("GA;900;{0};{1}", Client.Player.ID, character.ID));
            }
        }

        private void AcceptChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas.Substring(3), out charid))
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid) && Client.Player.State.ChallengeAsker == charid)
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                Client.Player.State.ChallengeAsked = -1;
                Client.Player.State.IsChallengeAsker = false;

                character.State.ChallengeAsker = -1;
                character.State.IsChallengeAsked = false;

                Client.Send(string.Format("GA;901;{0};{1}", Client.Player.ID, character.ID));
                character.NClient.Send(string.Format("GA;901;{0};{1}", character.ID, Client.Player.ID));

                Client.Player.GetMap().AddFight(new SunDofus.World.Game.Maps.Fights.Fight
                    (Client.Player, character, SunDofus.World.Game.Maps.Fights.Fight.FightType.Challenge, Client.Player.GetMap()));
            }
        }

        private void RefuseChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas.Substring(3), out charid))
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid) && Client.Player.State.ChallengeAsker == charid)
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                Client.Player.State.ChallengeAsked = -1;
                Client.Player.State.IsChallengeAsker = false;

                character.State.ChallengeAsker = -1;
                character.State.IsChallengeAsked = false;

                Client.Send(string.Format("GA;902;{0};{1}", Client.Player.ID, character.ID));
                character.NClient.Send(string.Format("GA;902;{0};{1}", character.ID, Client.Player.ID));
            }
        }

        #endregion

        #region Items

        private void DeleteItem(string datas)
        {
            var allDatas = datas.Split('|');
            var ID = 0;
            var quantity = 0;

            if (!int.TryParse(allDatas[0], out ID) || !int.TryParse(allDatas[1], out quantity) || quantity <= 0)
                return;

            Client.Player.ItemsInventary.DeleteItem(ID, quantity);
        }

        private void MoveItem(string datas)
        {
            var allDatas = datas.Split('|');

            var ID = 0;
            var pos = 0;
            var quantity = 1;

            if (allDatas.Length >= 3)
            {
                if (!int.TryParse(allDatas[2], out quantity))
                    return;
            }

            if (!int.TryParse(allDatas[0], out ID) || !int.TryParse(allDatas[1], out pos))
                return;

            Client.Player.ItemsInventary.MoveItem(ID, pos, quantity);
        }

        private void UseItem(string datas)
        {
            Client.Player.ItemsInventary.UseItem(datas);
        }

        #endregion

        #region StatsBoosts

        private void StatsBoosts(string datas)
        {
            var caract = 0;

            if (!int.TryParse(datas, out caract))
                return;

            var count = 0;

            switch (caract)
            {
                case 11:

                    if (Client.Player.CharactPoint < 1) 
                        return;

                    if (Client.Player.Class == 11)
                    {
                        Client.Player.Stats.life.Bases += 2;
                        Client.Player.Life += 2;
                    }
                    else
                    {
                        Client.Player.Stats.life.Bases += 1;
                        Client.Player.Life += 1;
                    }

                    Client.Player.CharactPoint -= 1;
                    Client.Player.SendChararacterStats();

                    break;

                case 12:

                    if (Client.Player.CharactPoint < 3) 
                        return;

                    Client.Player.Stats.wisdom.Bases += 1;
                    Client.Player.CharactPoint -= 3;
                    Client.Player.SendChararacterStats();

                    break;

                case 10:

                    if (Client.Player.Class == 1 | Client.Player.Class == 7 | Client.Player.Class == 2 | Client.Player.Class == 5)
                    {
                        if (Client.Player.Stats.strenght.Bases < 51) count = 2;
                        if (Client.Player.Stats.strenght.Bases > 50) count = 3;
                        if (Client.Player.Stats.strenght.Bases > 150) count = 4;
                        if (Client.Player.Stats.strenght.Bases > 250) count = 5;
                    }

                    else if (Client.Player.Class == 3 | Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.strenght.Bases < 51) count = 1;
                        if (Client.Player.Stats.strenght.Bases > 50) count = 2;
                        if (Client.Player.Stats.strenght.Bases > 150) count = 3;
                        if (Client.Player.Stats.strenght.Bases > 250) count = 4;
                        if (Client.Player.Stats.strenght.Bases > 350) count = 5;
                    }

                    else if (Client.Player.Class == 4 | Client.Player.Class == 6 | Client.Player.Class == 8 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.strenght.Bases < 101) count = 1;
                        if (Client.Player.Stats.strenght.Bases > 100) count = 2;
                        if (Client.Player.Stats.strenght.Bases > 200) count = 3;
                        if (Client.Player.Stats.strenght.Bases > 300) count = 4;
                        if (Client.Player.Stats.strenght.Bases > 400) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.strenght.Bases < 51) count = 1;
                        if (Client.Player.Stats.strenght.Bases > 50) count = 2;
                        if (Client.Player.Stats.strenght.Bases > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.strenght.Bases += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;

                case 15:

                    if (Client.Player.Class == 1 | Client.Player.Class == 2 | Client.Player.Class == 5 | Client.Player.Class == 7 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.intelligence.Bases < 101) count = 1;
                        if (Client.Player.Stats.intelligence.Bases > 100) count = 2;
                        if (Client.Player.Stats.intelligence.Bases > 200) count = 3;
                        if (Client.Player.Stats.intelligence.Bases > 300) count = 4;
                        if (Client.Player.Stats.intelligence.Bases > 400) count = 5;
                    }

                    else if (Client.Player.Class == 3)
                    {
                        if (Client.Player.Stats.intelligence.Bases < 21) count = 1;
                        if (Client.Player.Stats.intelligence.Bases > 20) count = 2;
                        if (Client.Player.Stats.intelligence.Bases > 60) count = 3;
                        if (Client.Player.Stats.intelligence.Bases > 100) count = 4;
                        if (Client.Player.Stats.intelligence.Bases > 140) count = 5;
                    }

                    else if (Client.Player.Class == 4)
                    {
                        if (Client.Player.Stats.intelligence.Bases < 51) count = 1;
                        if (Client.Player.Stats.intelligence.Bases > 50) count = 2;
                        if (Client.Player.Stats.intelligence.Bases > 150) count = 3;
                        if (Client.Player.Stats.intelligence.Bases > 250) count = 4;
                    }

                    else if (Client.Player.Class == 6 | Client.Player.Class == 8)
                    {
                        if (Client.Player.Stats.intelligence.Bases < 21) count = 1;
                        if (Client.Player.Stats.intelligence.Bases > 20) count = 2;
                        if (Client.Player.Stats.intelligence.Bases > 40) count = 3;
                        if (Client.Player.Stats.intelligence.Bases > 60) count = 4;
                        if (Client.Player.Stats.intelligence.Bases > 80) count = 5;
                    }

                    else if (Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.intelligence.Bases < 51) count = 1;
                        if (Client.Player.Stats.intelligence.Bases > 50) count = 2;
                        if (Client.Player.Stats.intelligence.Bases > 150) count = 3;
                        if (Client.Player.Stats.intelligence.Bases > 250) count = 4;
                        if (Client.Player.Stats.intelligence.Bases > 350) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.intelligence.Bases < 51) count = 1;
                        if (Client.Player.Stats.intelligence.Bases > 50) count = 2;
                        if (Client.Player.Stats.intelligence.Bases > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.intelligence.Bases += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;

                case 13:

                    if (Client.Player.Class == 1 | Client.Player.Class == 4 | Client.Player.Class == 5
                        | Client.Player.Class == 6 | Client.Player.Class == 7 | Client.Player.Class == 8 | Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.luck.Bases < 21) count = 1;
                        if (Client.Player.Stats.luck.Bases > 20) count = 2;
                        if (Client.Player.Stats.luck.Bases > 40) count = 3;
                        if (Client.Player.Stats.luck.Bases > 60) count = 4;
                        if (Client.Player.Stats.luck.Bases > 80) count = 5;
                    }

                    else if (Client.Player.Class == 2 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.luck.Bases < 101) count = 1;
                        if (Client.Player.Stats.luck.Bases > 100) count = 2;
                        if (Client.Player.Stats.luck.Bases > 200) count = 3;
                        if (Client.Player.Stats.luck.Bases > 300) count = 4;
                        if (Client.Player.Stats.luck.Bases > 400) count = 5;
                    }

                    else if (Client.Player.Class == 3)
                    {
                        if (Client.Player.Stats.luck.Bases < 101) count = 1;
                        if (Client.Player.Stats.luck.Bases > 100) count = 2;
                        if (Client.Player.Stats.luck.Bases > 150) count = 3;
                        if (Client.Player.Stats.luck.Bases > 230) count = 4;
                        if (Client.Player.Stats.luck.Bases > 330) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.luck.Bases < 51) count = 1;
                        if (Client.Player.Stats.luck.Bases > 50) count = 2;
                        if (Client.Player.Stats.luck.Bases > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.luck.Bases += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;

                case 14:

                    if (Client.Player.Class == 1 | Client.Player.Class == 2 | Client.Player.Class == 3 | Client.Player.Class == 5
                        | Client.Player.Class == 7 | Client.Player.Class == 8 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.agility.Bases < 21) count = 1;
                        if (Client.Player.Stats.agility.Bases > 20) count = 2;
                        if (Client.Player.Stats.agility.Bases > 40) count = 3;
                        if (Client.Player.Stats.agility.Bases > 60) count = 4;
                        if (Client.Player.Stats.agility.Bases > 80) count = 5;
                    }

                    else if (Client.Player.Class == 4)
                    {
                        if (Client.Player.Stats.agility.Bases < 101) count = 1;
                        if (Client.Player.Stats.agility.Bases > 100) count = 2;
                        if (Client.Player.Stats.agility.Bases > 200) count = 3;
                        if (Client.Player.Stats.agility.Bases > 300) count = 4;
                        if (Client.Player.Stats.agility.Bases > 400) count = 5;
                    }

                    else if (Client.Player.Class == 6 | Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.agility.Bases < 51) count = 1;
                        if (Client.Player.Stats.agility.Bases > 50) count = 2;
                        if (Client.Player.Stats.agility.Bases > 100) count = 3;
                        if (Client.Player.Stats.agility.Bases > 150) count = 4;
                        if (Client.Player.Stats.agility.Bases > 200) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.agility.Bases < 51) count = 1;
                        if (Client.Player.Stats.agility.Bases > 50) count = 2;
                        if (Client.Player.Stats.agility.Bases > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.agility.Bases += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;
            }
        }

        #endregion

        #region Spells

        private void SpellBoost(string datas)
        {
            var spellID = 0;

            if (!int.TryParse(datas, out spellID))
                return;

            if (!Client.Player.SpellsInventary.Spells.Any(x => x.ID == spellID))
            {
                Client.Send("SUE");
                return;
            }

            var level = Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Level;

            if (Client.Player.SpellPoint < level || level >= 6)
            {
                Client.Send("SUE");
                return;
            }

            Client.Player.SpellPoint -= level;

            Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Level++;

            Client.Send(string.Format("SUK{0}~{1}", spellID, level + 1));
            Client.Player.SendChararacterStats();
        }

        private void SpellMove(string _datas)
        {
            Client.Send("BN");

            var datas = _datas.Split('|');
            var spellID = 0;
            var newPos = 0;

            if (!int.TryParse(datas[0], out spellID) || !int.TryParse(datas[1], out newPos))
                return;

            if (!Client.Player.SpellsInventary.Spells.Any(x => x.ID == spellID))
                return;

            if (Client.Player.SpellsInventary.Spells.Any(x => x.Position == newPos))
            {
                Client.Player.SpellsInventary.Spells.First(x => x.Position == newPos).Position = 25;
                Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Position = newPos;
            }
            else
                Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Position = newPos;
        }

        #endregion

        #region Exchange

        private void ExchangeRequest(string datas)
        {
            if (Client.Player == null || Client.Player.State.Busy)
            {
                Client.Send("BN");
                return;
            }

            var packet = datas.Split('|');
            var ID = 0;
            var receiverID = 0;

            if (!int.TryParse(packet[0],out ID) || !int.TryParse(packet[1],out receiverID))
                return;

            switch (ID)
            {
                case 0://NPC BUY/SELL

                    var NPC = Client.Player.GetMap().Npcs.First(x => x.ID == receiverID);

                    if (NPC.Model.SellingList.Count == 0)
                    {
                        Client.Send("BN");
                        return;
                    }

                    Client.Player.State.OnExchange = true;
                    Client.Player.State.ActualNPC = NPC.ID;

                    Client.Send(string.Concat("ECK0|", NPC.ID));

                    var newPacket = "EL";

                    foreach (var i in NPC.Model.SellingList)
                    {
                        var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == i);
                        newPacket += string.Format("{0};{1}|", i, item.EffectInfos());
                    }

                    Client.Send(newPacket.Substring(0, newPacket.Length - 1));

                    break;

                case 1://Player

                    if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == receiverID))
                    {
                        var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == receiverID);

                        if (!character.IsConnected == true && !character.State.Busy)
                        {
                            Client.Send("BN");
                            return;
                        }

                        character.NClient.Send(string.Format("ERK{0}|{1}|1", Client.Player.ID, character.ID));
                        Client.Send(string.Format("ERK{0}|{1}|1", Client.Player.ID, character.ID));

                        character.State.ActualTraider = Client.Player.ID;
                        character.State.OnExchange = true;

                        Client.Player.State.ActualTraided = character.ID;
                        Client.Player.State.OnExchange = true;
                    }

                    break;
            }
        }

        private void CancelExchange(string t)
        {
            Client.Send("EV");

            if (Client.Player.State.OnExchange)
                SunDofus.World.Game.Exchanges.ExchangesManager.LeaveExchange(Client.Player);
            else if (Client.Player.State.OnExchangeWithBank)
                Client.Player.State.OnExchangeWithBank = false;
        }

        private void ExchangeBuy(string packet)
        {
            if (!Client.Player.State.OnExchange)
            {
                Client.Send("EBE");
                return;
            }

            var datas = packet.Split('|');
            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(datas[0], out itemID) || !int.TryParse(datas[1], out quantity))
                return;

            var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == itemID);
            var NPC = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.State.ActualNPC);

            if (quantity <= 0 || !NPC.Model.SellingList.Contains(itemID))
            {
                Client.Send("EBE");
                return;
            }

            var price = item.Price * quantity;

            if (Client.Player.Kamas >= price)
            {
                var newItem = new SunDofus.World.Game.Characters.Items.CharacterItem(item);
                newItem.GeneratItem(4);
                newItem.Quantity = quantity;


                Client.Player.Kamas -= price;
                Client.Send("EBK");
                Client.Player.ItemsInventary.AddItem(newItem, false);
            }
            else
                Client.Send("EBE");
        }

        private void ExchangeSell(string datas)
        {
            if (!Client.Player.State.Busy)
            {
                Client.Send("OSE");
                return;
            }

            var packet = datas.Split('|');

            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(packet[0], out itemID) || !int.TryParse(packet[1], out quantity))
                return;

            if (!Client.Player.ItemsInventary.ItemsList.Any(x => x.ID == itemID) || quantity <= 0)
            {
                Client.Send("OSE");
                return;
            }

            var item = Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);

            if (item.Quantity < quantity)
                quantity = item.Quantity;

            var price = Math.Floor((double)item.Model.Price / 10) * quantity;

            if (price < 1)
                price = 1;

            Client.Player.Kamas += (int)price;
            Client.Player.ItemsInventary.DeleteItem(item.ID, quantity);
            Client.Send("ESK");
        }

        private void ExchangeMove(string datas)
        {
            switch (datas[0])
            {
                case 'G': //kamas

                    if (Client.Player.State.OnExchangeWithBank)
                    {
                        var length = (long)0;
                        var addkamas = true;

                        if (!long.TryParse(datas.Substring(1), out length))
                            return;

                        if (datas[1] == '-')
                        {
                            addkamas = false;
                            if (!long.TryParse(datas.Substring(2), out length))
                                return;
                        }

                        World.Game.Bank.BanksManager.FindExchange(Client.Player).MoveKamas(length, addkamas);
                        return;
                    }

                    var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.ActualPlayerExchange);

                    if (!Client.Player.State.OnExchangePanel || !character.State.OnExchangePanel || character.State.ActualPlayerExchange != Client.Player.ID)
                    {
                        Client.Send("EME");
                        return;
                    }

                    var actualExchange = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                        x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character.ID));

                    var kamas = (long)0;

                    if (!long.TryParse(datas.Substring(1), out kamas))
                        return;

                    if (kamas > Client.Player.Kamas)
                        kamas = Client.Player.Kamas;
                    else if (kamas < 0)
                        kamas = 0;

                    actualExchange.MoveGold(Client.Player, kamas);

                    break;

                case 'O': //Items

                    var itemID = 0;
                    var quantity = 0;
                    var infos = new string[0];

                    if (Client.Player.State.OnExchangeWithBank)
                    {
                        var additem = true;
                        infos = datas.Substring(2).Split('|');

                        if (datas[1] == '-')
                            additem = false;

                        itemID = 0;
                        quantity = 0;
                        SunDofus.World.Game.Characters.Items.CharacterItem item = null;

                        if (!int.TryParse(infos[0], out itemID) || !int.TryParse(infos[1], out quantity))
                        {
                            Client.Send("EME");
                            return;
                        }

                        if (additem)
                        {
                            if (Client.Player.ItemsInventary.ItemsList.Any(x => x.ID == itemID))
                                item = Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);
                            else
                                return;
                        }
                        else
                        {
                            var bank = Game.Bank.BanksManager.FindExchange(Client.Player).Bank;

                            if (bank.Items.Any(x => x.ID == itemID))
                                item = bank.Items.First(x => x.ID == itemID);
                            else
                                return;
                        }

                        if (quantity <= 0)
                            quantity = 1;
                        else if (quantity > item.Quantity)
                            quantity = item.Quantity;

                        Game.Bank.BanksManager.FindExchange(Client.Player).MoveItem(item, quantity, additem);
                        return;
                    }

                    var character2 = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.ActualPlayerExchange);

                    if (!Client.Player.State.OnExchangePanel || !character2.State.OnExchangePanel || character2.State.ActualPlayerExchange != Client.Player.ID)
                    {
                        Client.Send("EME");
                        return;
                    }

                    var actualExchange2 = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                        x.memberTwo.Character.ID == character2.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character2.ID));

                    var add = (datas.Substring(1, 1) == "+" ? true : false);
                    infos = datas.Substring(2).Split('|');

                    itemID = 0;
                    quantity = 0;

                    if (!int.TryParse(infos[0], out itemID) || !int.TryParse(infos[1], out quantity))
                        return;

                    var charItem = Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);
                    if (charItem.Quantity < quantity)
                        quantity = charItem.Quantity;
                    if (quantity < 1)
                        return;

                    actualExchange2.MoveItem(Client.Player, charItem, quantity, add);

                    break;
            }
        }

        private void ExchangeAccept(string datas)
        {
            if (Client.Player.State.OnExchange && Client.Player.State.ActualTraider != -1)
            {
                var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.ActualTraider);
                if (character.State.ActualTraided == Client.Player.ID)
                {
                    SunDofus.World.Game.Exchanges.ExchangesManager.AddExchange(character, Client.Player);
                    return;
                }
            }
            Client.Send("BN");
        }

        private void ExchangeValidate(string datas)
        {
            if (!Client.Player.State.OnExchange)
            {
                Client.Send("BN");
                return;
            }

            Client.Player.State.OnExchangeAccepted = true;

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.ActualPlayerExchange);

            if (!Client.Player.State.OnExchangePanel || !character.State.OnExchangePanel || character.State.ActualPlayerExchange != Client.Player.ID)
            {
                Client.Send("EME");
                return;
            }

            var actualExchange = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character.ID));

            Client.Send(string.Concat("EK1", Client.Player.ID));
            character.NClient.Send(string.Concat("EK1", Client.Player.ID));

            if (character.State.OnExchangeAccepted)
                actualExchange.ValideExchange();
        }

        #endregion

        #region Party

        private void PartyInvite(string datas)
        {
            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas && x.IsConnected))
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas);
                if (character.State.Party != null || character.State.Busy)
                {
                    Client.Send(string.Concat("PIEa", datas));
                    return;
                }

                if (Client.Player.State.Party != null)
                {
                    if (Client.Player.State.Party.Members.Count < 8)
                    {
                        character.State.SenderInviteParty = Client.Player.ID;
                        character.State.OnWaitingParty = true;
                        Client.Player.State.ReceiverInviteParty = character.ID;
                        Client.Player.State.OnWaitingParty = true;

                        Client.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                        character.NClient.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                    }
                    else
                    {
                        Client.Send(string.Concat("PIEf", datas));
                        return;
                    }
                }
                else
                {
                    character.State.SenderInviteParty = Client.Player.ID;
                    character.State.OnWaitingParty = true;
                    Client.Player.State.ReceiverInviteParty = character.ID;
                    Client.Player.State.OnWaitingParty = true;

                    Client.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                    character.NClient.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                }
            }
            else
                Client.Send(string.Concat("PIEn", datas));
        }

        private void PartyRefuse(string datas)
        {
            if (Client.Player.State.SenderInviteParty == -1)
            {
                Client.Send("BN");
                return;
            }

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First
                (x => x.ID == Client.Player.State.SenderInviteParty);

            if (character.IsConnected == false || character.State.ReceiverInviteParty != Client.Player.ID)
            {
                Client.Send("BN");
                return;
            }

            character.State.ReceiverInviteParty = -1;
            character.State.OnWaitingParty = false;

            Client.Player.State.SenderInviteParty = -1;
            Client.Player.State.OnWaitingParty = false;

            character.NClient.Send("PR");
        }

        private void PartyAccept(string datas)
        {
            if (Client.Player.State.SenderInviteParty != -1 && Client.Player.State.OnWaitingParty)
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.SenderInviteParty);

                if (character.IsConnected == false || character.State.ReceiverInviteParty != Client.Player.ID)
                {
                    Client.Player.State.SenderInviteParty = -1;
                    Client.Player.State.OnWaitingParty = false;
                    Client.Send("BN");
                    return;
                }

                Client.Player.State.SenderInviteParty = -1;
                Client.Player.State.OnWaitingParty = false;

                character.State.ReceiverInviteParty = -1;
                character.State.OnWaitingParty = false;

                if (character.State.Party == null)
                {
                    character.State.Party = new CharacterParty(character);
                    character.State.Party.AddMember(Client.Player);
                }
                else
                {
                    if (character.State.Party.Members.Count > 7)
                    {
                        Client.Send("BN");
                        character.NClient.Send("PR");
                        return;
                    }
                    character.State.Party.AddMember(Client.Player);
                }

                character.NClient.Send("PR");
            }
            else
            {
                Client.Player.State.SenderInviteParty = -1;
                Client.Player.State.OnWaitingParty = false;
                Client.Send("BN");
            }
        }

        private void PartyLeave(string datas)
        {
            if (Client.Player.State.Party == null || !Client.Player.State.Party.Members.Keys.Contains(Client.Player))
            {
                Client.Send("BN");
                return;
            }

            if (datas == "")
                Client.Player.State.Party.LeaveParty(Client.Player.Name);
            else
            {
                var character = Client.Player.State.Party.Members.Keys.ToList().First(x => x.ID == int.Parse(datas));
                Client.Player.State.Party.LeaveParty(character.Name, Client.Player.ID.ToString());
            }
        }

        private void PartyFollow(string datas)
        {
            var add = (datas.Substring(0, 1) == "+" ? true : false);
            var charid = 0;

            if (!int.TryParse(datas.Substring(1, datas.Length - 1), out charid))
                return;

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.IsConnected || Client.Player.State.IsFollowing)
                {
                    Client.Send("BN");
                    return;
                }

                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player)
                    || character.State.Followers.Contains(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                lock(character.State.Followers)
                    character.State.Followers.Add(Client.Player);

                character.State.IsFollow = true;
                character.NClient.Send(string.Concat("Im052;", Client.Player.Name));

                Client.Player.State.FollowingID = character.ID;
                Client.Player.State.IsFollowing = true;

                Client.Send(string.Format("IC{0}|{1}", character.GetMap().Model.PosX, character.GetMap().Model.PosY));
                Client.Send(string.Concat("PF+", character.ID));
            }
            else
            {
                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player)
                    || !character.State.Followers.Contains(Client.Player) || character.ID != Client.Player.State.FollowingID)
                {
                    Client.Send("BN");
                    return;
                }

                lock (character.State.Followers)
                    character.State.Followers.Remove(Client.Player);

                character.State.IsFollow = false;
                character.NClient.Send(string.Concat("Im053;", Client.Player.Name));

                Client.Player.State.FollowingID = -1;
                Client.Player.State.IsFollowing = false;

                Client.Send("IC|");
                Client.Send("PF-");
            }
        }

        private void PartyGroupFollow(string datas)
        {
            var add = (datas.Substring(0, 1) == "+" ? true : false);
            var charid = 0;

            if (!int.TryParse(datas.Substring(1, datas.Length - 1), out charid))
                return;

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.IsConnected || character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.State.Party.Members.Keys.Where(x => x != character))
                {
                    if (charinparty.State.IsFollowing)
                        charinparty.NClient.Send("PF-");

                    lock (character.State.Followers)
                        character.State.Followers.Add(Client.Player);

                    character.NClient.Send(string.Concat("Im052;", Client.Player.Name));

                    charinparty.State.FollowingID = character.ID;
                    charinparty.State.IsFollowing = true;

                    charinparty.NClient.Send(string.Format("IC{0}|{1}", character.GetMap().Model.PosX, character.GetMap().Model.PosY));
                    charinparty.NClient.Send(string.Concat("PF+", character.ID));
                }

                character.State.IsFollow = true;
            }
            else
            {
                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.State.Party.Members.Keys.Where(x => x != character))
                {
                    lock (character.State.Followers)
                        character.State.Followers.Remove(Client.Player);

                    character.NClient.Send(string.Concat("Im053;", Client.Player.Name));

                    charinparty.State.FollowingID = -1;
                    charinparty.State.IsFollowing = false;

                    charinparty.NClient.Send("IC|");
                    charinparty.NClient.Send("PF-");
                }

                character.State.IsFollow = false;
            }
        }

        #endregion

        #region Dialogs

        private void DialogCreate(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            if ((!Client.Player.GetMap().Npcs.Any(x => x.ID == id) && Client.Player.GetMap().Collector.ID != id) || Client.Player.State.Busy)
            {
                Client.Send("BN");
                return;
            }

            if (Client.Player.GetMap().Npcs.Any(x => x.ID == id)) //Is also a NPC
            {
                var npc = Client.Player.GetMap().Npcs.First(x => x.ID == id);

                if (npc.Model.Question == null)
                {
                    Client.Send("BN");
                    Client.SendMessage("Dialogue inexistant !");
                    return;
                }

                Client.Player.State.OnDialoging = true;
                Client.Player.State.OnDialogingWith = npc.ID;

                Client.Send(string.Concat("DCK", npc.ID));

                var packet = string.Concat("DQ", npc.Model.Question.QuestionID);

                if(npc.Model.Question.Params.Count > 0)
                {
                    packet = string.Concat(packet, ";");

                    foreach (var param in npc.Model.Question.Params)
                        packet = string.Concat(Client.Player.GetParam(param), ",");

                    packet = packet.Substring(0, packet.Length - 1);
                }

                packet = string.Concat(packet, "|");

                if (npc.Model.Question.Answers.Count(x => x.HasConditions(Client.Player)) != 0)
                {
                    foreach (var answer in npc.Model.Question.Answers)
                    {
                        if (answer.HasConditions(Client.Player))
                            packet += string.Concat(answer.AnswerID, ";");
                    }
                }

                Client.Send(packet.Substring(0, packet.Length - 1));
            }
            else //Is also a collector
            {
                var collector = Client.Player.GetMap().Collector;

                Client.Player.State.OnDialoging = true;
                Client.Player.State.OnDialogingWith = collector.ID;
                Client.Send(string.Concat("DCK", collector.ID));

                var packet = string.Format("DQ1;{0},{1},{2},{3},{4}", 
                    collector.Guild.Name, collector.Guild.CollectorPods, collector.Guild.CollectorProspection, collector.Guild.CollectorWisdom, collector.Guild.Collectors.Count);

                Client.Send(packet);
            }
        }

        private void DialogReply(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[1], out id))
                return;

            if (!Client.Player.GetMap().Npcs.Any(x => x.ID == Client.Player.State.OnDialogingWith))
            {
                Client.Send("BN");
                return;
            }

            var npc = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.State.OnDialogingWith);

            if (!npc.Model.Question.Answers.Any(x => x.AnswerID == id))
            {
                Client.Send("BN");
                return;
            }

            var answer = npc.Model.Question.Answers.First(x => x.AnswerID == id);

            if (!answer.HasConditions(Client.Player))
            {
                Client.Send("BN");
                return;
            }

            answer.ApplyEffects(Client.Player);
            DialogExit("");
        }

        private void DialogExit(string datas)
        {
            Client.Send("DV");

            Client.Player.State.OnDialogingWith = -1;
            Client.Player.State.OnDialoging = false;
        }

        #endregion

        #region Fights



        #endregion

        #endregion
    }
}
