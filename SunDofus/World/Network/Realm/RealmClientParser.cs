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
    class RealmClientParser
    {
        public RealmClient Client;

        private delegate void Packets(string _string);
        private Dictionary<string, Packets> RegisteredPackets;

        public RealmClientParser(RealmClient client)
        {
            Client = client;

            RegisteredPackets = new Dictionary<string, Packets>();
            RegisterPackets();
        }

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

        #region Ticket

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
                }
                else
                    Client.Send("ATE");
            }
        }

        #endregion
        
        #region Character

        private void SendRandomName(string datas)
        {
            Client.Send(string.Format("APK{0}", Utilities.Basic.RandomName()));
        }

        private void SendCommunauty(string datas)
        {
            Client.Send(string.Format("AV{0}", Utilities.Config.GetIntElement("ServerCom")));
        }

        private void SendCharacterList(string datas)
        {
            string packet = string.Format("ALK{0}|{1}", Client.Infos.Subscription, Client.Characters.Count);

            if (Client.Characters.Count != 0)
            {
                foreach (SunDofus.World.Game.Characters.Character m_C in Client.Characters)
                    packet += string.Format("|{0}", m_C.PatternList());
            }

            Client.Send(packet);
        }

        private void CreateCharacter(string datas)
        {
            try
            {
                var characterDatas = datas.Split('|');

                if (characterDatas[0] != "" | CharactersManager.ExistsName(characterDatas[0]) == false)
                {
                    var character = new Character();

                    if (CharactersManager.CharactersList.Count > 0)
                        character.ID = (CharactersManager.CharactersList.OrderByDescending(x => x.ID).ToArray()[0].ID) + 1;
                    else
                        character.ID = 1;

                    character.Name = characterDatas[0];
                    character.Level = Utilities.Config.GetIntElement("StartLevel");
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
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Feca");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Feca");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Feca");
                            break;
                        case 2:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Osa");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Osa");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Osa");
                            break;
                        case 3:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Enu");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Enu");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Enu");
                            break;
                        case 4:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Sram");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Sram");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Sram");
                            break;
                        case 5:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Xel");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Xel");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Xel");
                            break;
                        case 6:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Eca");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Eca");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Eca");
                            break;
                        case 7:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Eni");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Eni");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Eni");
                            break;
                        case 8:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Iop");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Iop");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Iop");
                            break;
                        case 9:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Cra");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Cra");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Cra");
                            break;
                        case 10:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Sadi");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Sadi");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Sadi");
                            break;
                        case 11:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Sacri");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Sacri");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Sacri");
                            break;
                        case 12:
                            character.MapID = Utilities.Config.GetIntElement("StartMap_Panda");
                            character.MapCell = Utilities.Config.GetIntElement("StartCell_Panda");
                            character.Dir = Utilities.Config.GetIntElement("StartDir_Panda");
                            break;
                    }

                    character.CharactPoint = (character.Level - 1) * 5;
                    character.SpellPoint = (character.Level - 1);
                    character.Exp = Entities.Requests.LevelsRequests.ReturnLevel(character.Level).Character;
                    character.Kamas = (long)Utilities.Config.GetIntElement("StartKamas");

                    if (character.Class < 1 | character.Class > 12 | character.Sex < 0 | character.Sex > 1)
                    {
                        Client.Send("AAE");
                        return;
                    }

                    character.SpellsInventary.LearnSpells();
                    character.isNewCharacter = true;

                    lock(CharactersManager.CharactersList)
                        CharactersManager.CharactersList.Add(character);

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
                Utilities.Loggers.ErrorsLogger.Write(e.ToString());
            }
        }

        private void DeleteCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[0], out id))
                return;

            lock (CharactersManager.CharactersList)
            {
                if (!CharactersManager.CharactersList.Any(x => x.ID == id))
                    return;

                var character = CharactersManager.CharactersList.First(x => x.ID == id);

                if (datas.Split('|')[1] != Client.Infos.Answer && character.Level >= 20)
                {
                    Client.Send("ADE");
                    return;
                }

                lock(Client.Characters)
                    Client.Characters.Remove(character);

                Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedCharacterPacket().GetPacket(Client.Infos.ID, character.Name));
                character.isDeletedCharacter = true;

                SendCharacterList("");
            }
        }

        private void SelectCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            lock (CharactersManager.CharactersList)
            {
                if (!CharactersManager.CharactersList.Any(x => x.ID == id))
                    return;

                var character = CharactersManager.CharactersList.First(x => x.ID == id);

                lock (Client.Characters)
                {
                    if (Client.Characters.Contains(character))
                    {
                        Client.Player = character;
                        Client.Player.State = new CharacterState(Client.Player);
                        Client.Player.NetworkClient = Client;

                        Client.Player.isConnected = true;

                        foreach (var client in Network.ServersHandler.RealmServer.Clients.Where(x => x.Characters.Any(c => c.isConnected == true) && x.Friends.Contains(Client.Infos.Pseudo) && x.Player.Friends.willSeeWhenConnected))
                            client.Send(string.Concat("Im0143;", Client.Player.Name));

                        Client.Send(string.Format("ASK{0}", Client.Player.PatternSelect()));
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

        #region Realm

        private void SendDate(string datas)
        {
            Client.Send(string.Format("BD{0}", Utilities.Basic.GetDofusDate()));
        }

        private void CreateGame(string datas)
        {
            Client.Send(string.Format("GCK|1|{0}", Client.Player.Name));
            Client.Send("AR6bk");

            Client.Player.Channels.SendChannels();
            Client.Send("SLo+");
            Client.Player.SpellsInventary.SendAllSpells();
            Client.Send(string.Format("BT{0}", Utilities.Basic.GetActuelTime()));

            if (Client.Player.Life == 0)
            {
                Client.Player.UpdateStats();
                Client.Player.Life = Client.Player.MaximumLife;
            }

            Client.Player.ItemsInventary.RefreshBonus();
            Client.Player.SendPods();
            Client.Player.SendChararacterStats();

            Client.Player.LoadMap();

            Client.Send(string.Concat("FO", (Client.Player.Friends.willSeeWhenConnected ? "+" : "-")));
        }

        private void ChangeChannel(string channel)
        {
            char head;

            if (!char.TryParse(channel.Substring(1), out head))
                return;
            
            var state = (channel.Substring(0, 1) == "+" ? true : false);
            Client.Player.Channels.ChangeChannelState(head, state);
        }

        private void ChangeAlignmentEnable(string enable)
        {
            if (enable == "+")
            {
                if (Client.Player.Faction.ID != 0)
                    Client.Player.Faction.isEnabled = true;
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
                    Client.Player.Faction.isEnabled = false;
                    Client.Player.Faction.Honor -= hloose;
                }
            }

            Client.Player.SendChararacterStats();
        }

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
                Client.Player.Friends.willSeeWhenConnected = true;
            else
                Client.Player.Friends.willSeeWhenConnected = false;

            Client.Send(string.Concat("FO", (Client.Player.Friends.willSeeWhenConnected ? "+" : "-")));
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
                    //GuildMessage
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
                    GameMove(datas);
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

        private void GameMove(string datas)
        {
            var packet = datas.Substring(3);

            if (!Pathfinding.isValidCell(Client.Player.MapCell, packet))
            {
                Client.Send("GA;0");
                return;
            }

            var path = new Pathfinding(packet, Client.Player.GetMap(), Client.Player.MapCell, Client.Player.Dir);
            var newPath = path.RemakePath();

            newPath = path.GetStartPath + newPath;

            if (!Client.Player.GetMap().RushablesCells.Contains(path.Destination))
            {
                Client.Send("GA;0");
                return;
            }

            Client.Player.Dir = path.Direction;
            Client.Player.State.moveToCell = path.Destination;
            Client.Player.State.onMove = true;

            Client.Player.GetMap().Send(string.Format("GA0;1;{0};{1}", Client.Player.ID, newPath));
        }

        private void AskChallenge(string datas)
        {
            var charid = 0;

            if(!int.TryParse(datas.Substring(3), out charid))
                return;

            if (CharactersManager.CharactersList.Any(x => x.ID == charid))
            {
                var character = CharactersManager.CharactersList.First(x => x.ID == charid);

                if (Client.Player.State.Occuped || character.State.Occuped || Client.Player.GetMap().GetModel.ID != character.GetMap().GetModel.ID)
                {
                    Client.SendMessage("Personnage actuellement occupé ou indisponible !");
                    return;
                }

                Client.Player.State.ChallengeAsked = character.ID;
                Client.Player.State.isChallengeAsker = true;

                character.State.ChallengeAsker = Client.Player.ID;
                character.State.isChallengeAsked = true;

                Client.Player.GetMap().Send(string.Format("GA;900;{0};{1}", Client.Player.ID, character.ID));
            }
        }

        private void AcceptChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas.Substring(3), out charid))
                return;

            if (CharactersManager.CharactersList.Any(x => x.ID == charid) && Client.Player.State.ChallengeAsker == charid)
            {
                var character = CharactersManager.CharactersList.First(x => x.ID == charid);

                Client.Player.State.ChallengeAsked = -1;
                Client.Player.State.isChallengeAsker = false;

                character.State.ChallengeAsker = -1;
                character.State.isChallengeAsked = false;

                Client.Send(string.Format("GA;901;{0};{1}", Client.Player.ID, character.ID));
                character.NetworkClient.Send(string.Format("GA;901;{0};{1}", character.ID, Client.Player.ID));

                Client.Player.GetMap().AddFight(new SunDofus.World.Game.Maps.Fights.Fight
                    (Client.Player, character, SunDofus.World.Game.Maps.Fights.Fight.FightType.Challenge, Client.Player.GetMap()));
            }
        }

        private void RefuseChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas.Substring(3), out charid))
                return;

            if (CharactersManager.CharactersList.Any(x => x.ID == charid) && Client.Player.State.ChallengeAsker == charid)
            {
                var character = CharactersManager.CharactersList.First(x => x.ID == charid);

                Client.Player.State.ChallengeAsked = -1;
                Client.Player.State.isChallengeAsker = false;

                character.State.ChallengeAsker = -1;
                character.State.isChallengeAsked = false;

                Client.Send(string.Format("GA;902;{0};{1}", Client.Player.ID, character.ID));
                character.NetworkClient.Send(string.Format("GA;902;{0};{1}", character.ID, Client.Player.ID));
            }
        }

        private void EndAction(string datas)
        {
            switch(datas[0])
            {
                case 'K':

                    if (Client.Player.State.onMove == true)
                    {
                        Client.Player.State.onMove = false;
                        Client.Player.MapCell = Client.Player.State.moveToCell;
                        Client.Player.State.moveToCell = -1;
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
                    
                    Client.Player.State.onMove = false;
                    Client.Player.MapCell = cell;

                    return;
            }
        }

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

            Client.Player.ItemsInventary.MoveItem(int.Parse(allDatas[0]), int.Parse(allDatas[1]), (allDatas.Length >= 3 ? int.Parse(allDatas[2]) : 1));
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
            if (Client.Player == null || Client.Player.State.Occuped)
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

                    Client.Player.State.onExchange = true;
                    Client.Player.State.actualNPC = NPC.ID;

                    Client.Send(string.Format("ECK0|{0}", NPC.ID));

                    var newPacket = "EL";

                    foreach (var i in NPC.Model.SellingList)
                    {
                        var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == i);
                        newPacket += string.Format("{0};{1}|", i, item.EffectInfos());
                    }

                    Client.Send(newPacket.Substring(0, newPacket.Length - 1));

                    break;

                case 1://Player

                    if (SunDofus.World.Game.Characters.CharactersManager.CharactersList.Any(x => x.ID == receiverID))
                    {
                        var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == receiverID);

                        if (!character.isConnected == true && !character.State.Occuped)
                        {
                            Client.Send("BN");
                            return;
                        }

                        character.NetworkClient.Send(string.Format("ERK{0}|{1}|1", Client.Player.ID, character.ID));
                        Client.Send(string.Format("ERK{0}|{1}|1", Client.Player.ID, character.ID));

                        character.State.actualTraider = Client.Player.ID;
                        character.State.onExchange = true;

                        Client.Player.State.actualTraided = character.ID;
                        Client.Player.State.onExchange = true;
                    }

                    break;
            }
        }

        private void CancelExchange(string t)
        {
            Client.Send("EV");

            if (Client.Player.State.onExchange)
                SunDofus.World.Game.Exchanges.ExchangesManager.LeaveExchange(Client.Player);
        }

        private void ExchangeBuy(string packet)
        {
            if (!Client.Player.State.onExchange)
            {
                Client.Send("OBE");
                return;
            }

            var datas = packet.Split('|');
            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(datas[0], out itemID) || int.TryParse(datas[1], out quantity))
                return;

            var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == itemID);
            var NPC = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.State.actualNPC);

            if (quantity <= 0 || !NPC.Model.SellingList.Contains(itemID))
            {
                Client.Send("OBE");
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
                Client.Send("OBE");
        }

        private void ExchangeSell(string datas)
        {
            if (!Client.Player.State.Occuped)
            {
                Client.Send("OSE");
                return;
            }

            var packet = datas.Split('|');

            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(packet[0], out itemID) || int.TryParse(packet[1], out quantity))
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

                    var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == Client.Player.State.actualPlayerExchange);

                    if (!Client.Player.State.onExchangePanel || !character.State.onExchangePanel || character.State.actualPlayerExchange != Client.Player.ID)
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

                    var character2 = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == Client.Player.State.actualPlayerExchange);

                    if (!Client.Player.State.onExchangePanel || !character2.State.onExchangePanel || character2.State.actualPlayerExchange != Client.Player.ID)
                    {
                        Client.Send("EME");
                        return;
                    }

                    var actualExchange2 = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                        x.memberTwo.Character.ID == character2.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character2.ID));

                    var add = (datas.Substring(1, 1) == "+" ? true : false);
                    var infos = datas.Substring(2).Split('|');

                    var itemID = 0;
                    var quantity = 0;

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
            if (Client.Player.State.onExchange && Client.Player.State.actualTraider != -1)
            {
                var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == Client.Player.State.actualTraider);
                if (character.State.actualTraided == Client.Player.ID)
                {
                    SunDofus.World.Game.Exchanges.ExchangesManager.AddExchange(character, Client.Player);
                    return;
                }
            }
            Client.Send("BN");
        }

        private void ExchangeValidate(string datas)
        {
            if (!Client.Player.State.onExchange)
            {
                Client.Send("BN");
                return;
            }

            Client.Player.State.onExchangeAccepted = true;

            var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == Client.Player.State.actualPlayerExchange);

            if (!Client.Player.State.onExchangePanel || !character.State.onExchangePanel || character.State.actualPlayerExchange != Client.Player.ID)
            {
                Client.Send("EME");
                return;
            }

            var actualExchange = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character.ID));

            Client.Send(string.Format("EK1{0}", Client.Player.ID));
            character.NetworkClient.Send(string.Format("EK1{0}", Client.Player.ID));

            if (character.State.onExchangeAccepted)
                actualExchange.ValideExchange();
        }

        #endregion

        #region Party

        private void PartyInvite(string datas)
        {
            if (SunDofus.World.Game.Characters.CharactersManager.CharactersList.Any(x => x.Name == datas && x.isConnected))
            {
                var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.Name == datas);
                if (character.State.Party != null || character.State.Occuped)
                {
                    Client.Send(string.Format("PIEa{0}", datas));
                    return;
                }

                if (Client.Player.State.Party != null)
                {
                    if (Client.Player.State.Party.Members.Count < 8)
                    {
                        character.State.senderInviteParty = Client.Player.ID;
                        character.State.onWaitingParty = true;
                        Client.Player.State.receiverInviteParty = character.ID;
                        Client.Player.State.onWaitingParty = true;

                        Client.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                        character.NetworkClient.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                    }
                    else
                    {
                        Client.Send(string.Format("PIEf{0}", datas));
                        return;
                    }
                }
                else
                {
                    character.State.senderInviteParty = Client.Player.ID;
                    character.State.onWaitingParty = true;
                    Client.Player.State.receiverInviteParty = character.ID;
                    Client.Player.State.onWaitingParty = true;

                    Client.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                    character.NetworkClient.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                }
            }
            else
                Client.Send(string.Format("PIEn{0}", datas));
        }

        private void PartyRefuse(string datas)
        {
            if (Client.Player.State.senderInviteParty == -1)
            {
                Client.Send("BN");
                return;
            }

            var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First
                (x => x.ID == Client.Player.State.senderInviteParty);

            if (character.isConnected == false || character.State.receiverInviteParty != Client.Player.ID)
            {
                Client.Send("BN");
                return;
            }

            character.State.receiverInviteParty = -1;
            character.State.onWaitingParty = false;

            Client.Player.State.senderInviteParty = -1;
            Client.Player.State.onWaitingParty = false;

            character.NetworkClient.Send("PR");
        }

        private void PartyAccept(string datas)
        {
            if (Client.Player.State.senderInviteParty != -1 && Client.Player.State.onWaitingParty)
            {
                var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == Client.Player.State.senderInviteParty);

                if (character.isConnected == false || character.State.receiverInviteParty != Client.Player.ID)
                {
                    Client.Player.State.senderInviteParty = -1;
                    Client.Player.State.onWaitingParty = false;
                    Client.Send("BN");
                    return;
                }

                Client.Player.State.senderInviteParty = -1;
                Client.Player.State.onWaitingParty = false;

                character.State.receiverInviteParty = -1;
                character.State.onWaitingParty = false;

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
                        character.NetworkClient.Send("PR");
                        return;
                    }
                    character.State.Party.AddMember(Client.Player);
                }

                character.NetworkClient.Send("PR");
            }
            else
            {
                Client.Player.State.senderInviteParty = -1;
                Client.Player.State.onWaitingParty = false;
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

            var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.isConnected || Client.Player.State.isFollowing)
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

                character.State.isFollow = true;
                character.NetworkClient.Send(string.Format("Im052;{0}", Client.Player.Name));

                Client.Player.State.followingID = character.ID;
                Client.Player.State.isFollowing = true;

                Client.Send(string.Format("IC{0}|{1}", character.GetMap().GetModel.PosX, character.GetMap().GetModel.PosY));
                Client.Send(string.Format("PF+{0}", character.ID));
            }
            else
            {
                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player)
                    || !character.State.Followers.Contains(Client.Player) || character.ID != Client.Player.State.followingID)
                {
                    Client.Send("BN");
                    return;
                }

                lock (character.State.Followers)
                    character.State.Followers.Remove(Client.Player);

                character.State.isFollow = false;
                character.NetworkClient.Send(string.Format("Im053;{0}", Client.Player.Name));

                Client.Player.State.followingID = -1;
                Client.Player.State.isFollowing = false;

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

            var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.isConnected || character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.State.Party.Members.Keys.Where(x => x != character))
                {
                    if (charinparty.State.isFollowing)
                        charinparty.NetworkClient.Send("PF-");

                    lock (character.State.Followers)
                        character.State.Followers.Add(Client.Player);

                    character.NetworkClient.Send(string.Format("Im052;{0}", Client.Player.Name));

                    charinparty.State.followingID = character.ID;
                    charinparty.State.isFollowing = true;

                    charinparty.NetworkClient.Send(string.Format("IC{0}|{1}", character.GetMap().GetModel.PosX, character.GetMap().GetModel.PosY));
                    charinparty.NetworkClient.Send(string.Format("PF+{0}", character.ID));
                }

                character.State.isFollow = true;
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

                    character.NetworkClient.Send(string.Format("Im053;{0}", Client.Player.Name));

                    charinparty.State.followingID = -1;
                    charinparty.State.isFollowing = false;

                    charinparty.NetworkClient.Send("IC|");
                    charinparty.NetworkClient.Send("PF-");
                }

                character.State.isFollow = false;
            }
        }

        #endregion

        #region Dialogs

        private void DialogCreate(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            if (!Client.Player.GetMap().Npcs.Any(x => x.ID == id) || Client.Player.State.Occuped)
            {
                Client.Send("BN");
                return;
            }

            var npc = Client.Player.GetMap().Npcs.First(x => x.ID == id);

            if (npc.Model.Question == null)
            {
                Client.Send("BN");
                Client.SendMessage("Dialogue inexistant !");
                return;
            }

            Client.Player.State.onDialoging = true;
            Client.Player.State.onDialogingWith = npc.ID;

            Client.Send(string.Format("DCK{0}", npc.ID));

            if (npc.Model.Question.Answers.Count(x => x.HasConditions(Client.Player)) == 0)
                Client.Send(string.Format("DQ{0}", npc.Model.Question.QuestionID));
            else
            {
                var packet = string.Format("DQ{0}|", npc.Model.Question.QuestionID);

                foreach (var answer in npc.Model.Question.Answers)
                {
                    if (answer.HasConditions(Client.Player))
                        packet += string.Format("{0};", answer.AnswerID);
                }

                Client.Send(packet.Substring(0, packet.Length - 1));
            }
        }

        private void DialogReply(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[1], out id))
                return;

            if (!Client.Player.GetMap().Npcs.Any(x => x.ID == Client.Player.State.onDialogingWith))
            {
                Client.Send("BN");
                return;
            }

            var npc = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.State.onDialogingWith);

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

            Client.Player.State.onDialogingWith = -1;
            Client.Player.State.onDialoging = false;
        }

        #endregion

        #region Fights



        #endregion

        #endregion
    }
}
