﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus;
using SunDofus.World.Game.Characters.Stats;
using SunDofus.World.Game.Characters.Spells;
using SunDofus.World.Game.Characters.Items;
using SunDofus.World.Network.Realm;

namespace SunDofus.World.Game.Characters
{
    class Character
    {
        private string _name;
        private int _ID, _color, _color2, _color3, _class, _sex, _skin, _size, _level, _mapID, _mapCell, _dir, _charactPoint, _spellPoint, _energy;
        private int _maximumLife, _life, _pods, _savemap, _savecell;
        private long _exp, _kamas;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

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
        public int Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        public int Color2
        {
            get
            {
                return _color2;
            }
            set
            {
                _color2 = value;
            }
        }
        public int Color3
        {
            get
            {
                return _color3;
            }
            set
            {
                _color3 = value;
            }
        }
        public int Class
        {
            get
            {
                return _class;
            }
            set
            {
                _class = value;
            }
        }
        public int Sex
        {
            get
            {
                return _sex;
            }
            set
            {
                _sex = value;
            }
        }
        public int Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                _skin = value;
            }
        }
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
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
        public int MapID
        {
            get
            {
                return _mapID;
            }
            set
            {
                _mapID = value;
            }
        }
        public int MapCell
        {
            get
            {
                return _mapCell;
            }
            set
            {
                _mapCell = value;
            }
        }
        public int Dir
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }
        public int CharactPoint
        {
            get
            {
                return _charactPoint;
            }
            set
            {
                _charactPoint = value;
            }
        }
        public int SpellPoint
        {
            get
            {
                return _spellPoint;
            }
            set
            {
                _spellPoint = value;
            }
        }
        public int Energy
        {
            get
            {
                return _energy;
            }
            set
            {
                _energy = value;
            }
        }
        public int MaximumLife
        {
            get
            {
                return _maximumLife;
            }
            set
            {
                _maximumLife = value;
            }
        }
        public int Life
        {
            get
            {
                return _life;
            }
            set
            {
                _life = value;
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
        public int SaveMap
        {
            get
            {
                return _savemap;
            }
            set
            {
                _savemap = value;
            }
        }
        public int SaveCell
        {
            get
            {
                return _savecell;
            }
            set
            {
                _savecell = value;
            }
        }

        public long Exp
        {
            get
            {
                return _exp;
            }
            set
            {
                _exp = value;
            }
        }
        public long Kamas
        {
            get
            {
                return _kamas;
            }
            set
            {
                _kamas = value;
            }
        }

        public bool isNewCharacter;
        public bool isDeletedCharacter;
        public bool isConnected;

        private long QuotaRecruitment;
        private long QuotaTrade;

        public List<int> Zaaps;

        public Stats.Stats Stats;
        public Guilds.Guild Guild;
        public InventaryItems ItemsInventary;
        public InventarySpells SpellsInventary;
        public RealmClient NetworkClient;

        public CharacterState State;
        public CharacterFaction Faction;
        public CharacterChannels Channels;
        public CharacterJobs Jobs;

        public CharacterFriends Friends;
        public CharacterEnemies Enemies;

        public Character()
        {
            Zaaps = new List<int>();

            Stats = new Stats.Stats();
            ItemsInventary = new InventaryItems(this);
            SpellsInventary = new InventarySpells(this);
            Faction = new CharacterFaction(this);
            Jobs = new CharacterJobs(this);

            Channels = new CharacterChannels(this);
            Friends = new CharacterFriends(this);
            Enemies = new CharacterEnemies(this);

            Energy = 10000;
            isConnected = false;
            isDeletedCharacter = false;

            QuotaRecruitment = 0;
            QuotaTrade = 0;
        }

        #region Exp

        public void AddExp(long exp)
        {
            Exp += exp;
            LevelUp();
        }

        private void LevelUp()
        {
            if (this.Level == Entities.Requests.LevelsRequests.MaxLevel())
                return;

            if (Exp >= Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character)
            {
                while (Exp >= Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character)
                {
                    if (this.Level == Entities.Requests.LevelsRequests.MaxLevel())
                        break;

                    Level++;
                    SpellPoint++;
                    CharactPoint += 5;
                }

                NetworkClient.Send(string.Format("AN{0}", Level));
                SpellsInventary.LearnSpells();
                SendChararacterStats();
            }
        }

        #endregion

        #region ChatSpam

        public long TimeTrade()
        {
            return (long)Math.Ceiling((double)((QuotaTrade - Environment.TickCount) / 1000));
        }

        public long TimeRecruitment()
        {
            return (long)Math.Ceiling((double)((QuotaRecruitment - Environment.TickCount) / 1000));
        }

        public bool CanSendinTrade()
        {
            return (TimeTrade() <= 0 ? true : false);
        }

        public bool CanSendinRecruitment()
        {
            return (TimeRecruitment() <= 0 ? true : false);
        }

        public void RefreshTrade()
        {
            QuotaTrade = Environment.TickCount + Utilities.Config.GetLongElement("AntiSpamTrade");
        }

        public void RefreshRecruitment()
        {
            QuotaRecruitment = Environment.TickCount + Utilities.Config.GetLongElement("AntiSpamRecruitment");
        }

        #endregion

        #region Items

        public string GetItemsPos()
        {
            var packet = "";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 1))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 1).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 6))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 6).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 7))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 7).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 8))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 8).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 15))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 15).Model.ID);

            return packet;
        }

        public string GetItems()
        {
            return string.Join(";", ItemsInventary.ItemsList);
        }

        public string GetItemsToSave()
        {
            return (string.Join(";", from x in ItemsInventary.ItemsList select x.SaveString()));
        }

#endregion

        #region Pattern

        public string PatternList()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append("0;").Append(Utilities.Config.GetIntElement("ServerId")).Append(";;;");
            }

            return builder.ToString();
        }

        public string PatternOnParty()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Color).Append(";");
                builder.Append(Color2).Append(";");
                builder.Append(Color3).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append(Life).Append(",").Append(MaximumLife).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Stats.initiative).Append(";");
                builder.Append(Stats.prospection).Append(";0");
            }

            return builder.ToString();
        }

        public string PatternSelect()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append("|").Append(ID).Append("|");
                builder.Append(Name).Append("|");
                builder.Append(Level).Append("|");
                builder.Append(Class).Append("|");
                builder.Append(Skin).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append("||");
                builder.Append(GetItems()).Append("|");
            }

            return builder.ToString();
        }

        public string PatternGuild()
        {
            var member = Guild.Members.First(x => x.Character == this);

            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(member.Rank).Append(";");
                builder.Append(member.ExpGaved).Append(";");
                builder.Append(member.ExpGived).Append(";");
                builder.Append(member.Rights).Append(";");
                builder.Append((isConnected ? "1" : "0")).Append(";");
                builder.Append(Faction.ID).Append(";0");
            }

            return builder.ToString();
        }

        public string PatternDisplayChar()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(MapCell).Append(";");
                builder.Append(Dir).Append(";0;");
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Class).Append(";");
                builder.Append(Skin).Append("^").Append(Size).Append(";");
                builder.Append(Sex).Append(";").Append(Faction.AlignementInfos).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";"); // Items
                builder.Append("0;"); //Aura
                builder.Append(";;");
                builder.Append(";"); // Guild
                builder.Append(";0;");
                builder.Append(";"); // Mount
            }

            return builder.ToString();
        }

        #endregion

        #region Map

        public void LoadMap()
        {
            if (Entities.Requests.MapsRequests.MapsList.Any(x => x.Model.ID == this.MapID))
            {
                var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == this.MapID);

                NetworkClient.Send(string.Format("GDM|{0}|{1}|{2}", map.Model.ID, map.Model.Date, map.Model.Key));

                if (this.State.IsFollow)
                {
                    foreach (var character in this.State.Followers)
                        character.NetworkClient.Send(string.Format("IC{0}|{1}", GetMap().Model.PosX, GetMap().Model.PosY));
                }
            }
        }

        public bool isInIncarnam
        {
            get
            {
                return GetMap().Model.SubArea == 440 || GetMap().Model.SubArea == 442 || GetMap().Model.SubArea == 443 ||
                    GetMap().Model.SubArea == 444 || GetMap().Model.SubArea == 445 || GetMap().Model.SubArea == 446 ||
                    GetMap().Model.SubArea == 449 || GetMap().Model.SubArea == 450;
            }
        }

        public void TeleportNewMap(int _mapID, int _cell)
        {
            NetworkClient.Send(string.Format("GA;2;{0};", ID));

            GetMap().DelPlayer(this);
            var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == _mapID);

            MapID = map.Model.ID;
            MapCell = _cell;

            LoadMap();
        }

        public Maps.Map GetMap()
        {
            return Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == this.MapID);
        }

        #endregion

        #region Stats

        public void SendChararacterStats()
        {
            UpdateStats();
            NetworkClient.Send(string.Format("As{0}", this.ToString()));
        }

        public void SendPods()
        {
            NetworkClient.Send(string.Format("Ow{0}|{1}", Pods, Stats.maxPods.Total()));
        }

        public void ResetBonus()
        {
            Stats.life.Boosts = 0;
            Stats.wisdom.Boosts = 0;
            Stats.strenght.Boosts = 0;
            Stats.intelligence.Boosts = 0;
            Stats.luck.Boosts = 0;
            Stats.agility.Boosts = 0;

            Stats.initiative.Boosts = 0;
            Stats.prospection.Boosts = 0;
            Stats.PO.Boosts = 0;
            Stats.PA.Boosts = 0;
            Stats.PM.Boosts = 0;
            Stats.maxMonsters.Boosts = 0;
            Stats.maxPods.Boosts = 0;

            Stats.bonusDamage.Boosts = 0;
            Stats.returnDamage.Boosts = 0;
            Stats.bonusDamagePercent.Boosts = 0;
            Stats.bonusDamagePhysic.Boosts = 0;
            Stats.bonusDamageMagic.Boosts = 0;
            Stats.bonusHeal.Boosts = 0;
            Stats.bonusDamageTrap.Boosts = 0;
            Stats.bonusDamageTrapPercent.Boosts = 0;
            Stats.bonusCritical.Boosts = 0;
            Stats.bonusFail.Boosts = 0;

            Stats.armorNeutral.Boosts = 0;
            Stats.armorPercentNeutral.Boosts = 0;
            Stats.armorPvpNeutral.Boosts = 0;
            Stats.armorPvpPercentNeutral.Boosts = 0;

            Stats.armorIntelligence.Boosts = 0;
            Stats.armorPercentIntelligence.Boosts = 0;
            Stats.armorPvpIntelligence.Boosts = 0;
            Stats.armorPvpPercentIntelligence.Boosts = 0;

            Stats.armorStrenght.Boosts = 0;
            Stats.armorPercentStrenght.Boosts = 0;
            Stats.armorPvpStrenght.Boosts = 0;
            Stats.armorPvpPercentStrenght.Boosts = 0;

            Stats.armorLuck.Boosts = 0;
            Stats.armorPercentLuck.Boosts = 0;
            Stats.armorPvpLuck.Boosts = 0;
            Stats.armorPvpPercentLuck.Boosts = 0;

            Stats.armorAgility.Boosts = 0;
            Stats.armorPercentAgility.Boosts = 0;
            Stats.armorPvpAgility.Boosts = 0;
            Stats.armorPvpPercentAgility.Boosts = 0;
        }

        public void ResetItemsStats()
        {
            Stats.life.Items = 0;
            Stats.wisdom.Items = 0;
            Stats.strenght.Items = 0;
            Stats.intelligence.Items = 0;
            Stats.luck.Items = 0;
            Stats.agility.Items = 0;

            Stats.initiative.Items = 0;
            Stats.prospection.Items = 0;
            Stats.PO.Items = 0;
            Stats.PA.Items = 0;
            Stats.PM.Items = 0;
            Stats.maxMonsters.Items = 0;
            Stats.maxPods.Items = 0;

            Stats.dodgePA.Items = 0;
            Stats.dodgePM.Items = 0;

            Stats.bonusDamage.Items = 0;
            Stats.returnDamage.Items = 0;
            Stats.bonusDamagePercent.Items = 0;
            Stats.bonusDamagePhysic.Items = 0;
            Stats.bonusDamageMagic.Items = 0;
            Stats.bonusHeal.Items = 0;
            Stats.bonusDamageTrap.Items = 0;
            Stats.bonusDamageTrapPercent.Items = 0;
            Stats.bonusCritical.Items = 0;
            Stats.bonusFail.Items = 0;

            Stats.armorNeutral.Items = 0;
            Stats.armorPercentNeutral.Items = 0;
            Stats.armorPvpNeutral.Items = 0;
            Stats.armorPvpPercentNeutral.Items = 0;

            Stats.armorIntelligence.Items = 0;
            Stats.armorPercentIntelligence.Items = 0;
            Stats.armorPvpIntelligence.Items = 0;
            Stats.armorPvpPercentIntelligence.Items = 0;

            Stats.armorStrenght.Items = 0;
            Stats.armorPercentStrenght.Items = 0;
            Stats.armorPvpStrenght.Items = 0;
            Stats.armorPvpPercentStrenght.Items = 0;

            Stats.armorLuck.Items = 0;
            Stats.armorPercentLuck.Items = 0;
            Stats.armorPvpLuck.Items = 0;
            Stats.armorPvpPercentLuck.Items = 0;

            Stats.armorAgility.Items = 0;
            Stats.armorPercentAgility.Items = 0;
            Stats.armorPvpAgility.Items = 0;
            Stats.armorPvpPercentAgility.Items = 0;
        }

        public void ResetDons()
        {
            Stats.life.Dons = 0;
            Stats.wisdom.Dons = 0;
            Stats.strenght.Dons = 0;
            Stats.intelligence.Dons = 0;
            Stats.luck.Dons = 0;
            Stats.agility.Dons = 0;

            Stats.initiative.Dons = 0;
            Stats.prospection.Dons = 0;
            Stats.PO.Dons = 0;
            Stats.PA.Dons = 0;
            Stats.PM.Dons = 0;
            Stats.maxMonsters.Dons = 0;
            Stats.maxPods.Dons = 0;

            Stats.bonusDamage.Dons = 0;
            Stats.returnDamage.Dons = 0;
            Stats.bonusDamagePercent.Dons = 0;
            Stats.bonusDamagePhysic.Dons = 0;
            Stats.bonusDamageMagic.Dons = 0;
            Stats.bonusHeal.Dons = 0;
            Stats.bonusDamageTrap.Dons = 0;
            Stats.bonusDamageTrapPercent.Dons = 0;
            Stats.bonusCritical.Dons = 0;
            Stats.bonusFail.Dons = 0;

            Stats.armorNeutral.Dons = 0;
            Stats.armorPercentNeutral.Dons = 0;
            Stats.armorPvpNeutral.Dons = 0;
            Stats.armorPvpPercentNeutral.Dons = 0;

            Stats.armorIntelligence.Dons = 0;
            Stats.armorPercentIntelligence.Dons = 0;
            Stats.armorPvpIntelligence.Dons = 0;
            Stats.armorPvpPercentIntelligence.Dons = 0;

            Stats.armorStrenght.Dons = 0;
            Stats.armorPercentStrenght.Dons = 0;
            Stats.armorPvpStrenght.Dons = 0;
            Stats.armorPvpPercentStrenght.Dons = 0;

            Stats.armorLuck.Dons = 0;
            Stats.armorPercentLuck.Dons = 0;
            Stats.armorPvpLuck.Dons = 0;
            Stats.armorPvpPercentLuck.Dons = 0;

            Stats.armorAgility.Dons = 0;
            Stats.armorPercentAgility.Dons = 0;
            Stats.armorPvpAgility.Dons = 0;
            Stats.armorPvpPercentAgility.Dons = 0;
        }

        public void ResetStats()
        {
            Stats.life.Bases = 0;
            Stats.wisdom.Bases = 0;
            Stats.strenght.Bases = 0;
            Stats.intelligence.Bases = 0;
            Stats.luck.Bases = 0;
            Stats.agility.Bases = 0;

            Stats.initiative.Bases = 0;
            Stats.prospection.Bases = 0;
            Stats.PO.Bases = 0;
            Stats.PA.Bases = 0;
            Stats.PM.Bases = 0;
            Stats.maxMonsters.Bases = 0;
            Stats.maxPods.Bases = 0;

            Stats.bonusDamage.Bases = 0;
            Stats.returnDamage.Bases = 0;
            Stats.bonusDamagePercent.Bases = 0;
            Stats.bonusDamagePhysic.Bases = 0;
            Stats.bonusDamageMagic.Bases = 0;
            Stats.bonusHeal.Bases = 0;
            Stats.bonusDamageTrap.Bases = 0;
            Stats.bonusDamageTrapPercent.Bases = 0;
            Stats.bonusCritical.Bases = 0;
            Stats.bonusFail.Bases = 0;

            Stats.armorNeutral.Bases = 0;
            Stats.armorPercentNeutral.Bases = 0;
            Stats.armorPvpNeutral.Bases = 0;
            Stats.armorPvpPercentNeutral.Bases = 0;

            Stats.armorIntelligence.Bases = 0;
            Stats.armorPercentIntelligence.Bases = 0;
            Stats.armorPvpIntelligence.Bases = 0;
            Stats.armorPvpPercentIntelligence.Bases = 0;

            Stats.armorStrenght.Bases = 0;
            Stats.armorPercentStrenght.Bases = 0;
            Stats.armorPvpStrenght.Bases = 0;
            Stats.armorPvpPercentStrenght.Bases = 0;

            Stats.armorLuck.Bases = 0;
            Stats.armorPercentLuck.Bases = 0;
            Stats.armorPvpLuck.Bases = 0;
            Stats.armorPvpPercentLuck.Bases = 0;

            Stats.armorAgility.Bases = 0;
            Stats.armorPercentAgility.Bases = 0;
            Stats.armorPvpAgility.Bases = 0;
            Stats.armorPvpPercentAgility.Bases = 0;
        }

        public void AddLife(int _life)
        {
            if (Life == MaximumLife)
                NetworkClient.SendMessage("Im119");

            else if ((Life + _life) > MaximumLife)
            {
                NetworkClient.SendMessage(string.Format("Im01;{0}", (MaximumLife - Life)));
                Life = MaximumLife;
            }
            else
            {
                NetworkClient.SendMessage(string.Format("Im01;{0}", _life));
                Life += _life;
            }
        }

        public void UpdateStats()
        {
            var dif = 0;

            if (Life < MaximumLife)
                dif = MaximumLife - Life;

            MaximumLife = Stats.life.Total() + (NetworkClient.Player.Level * 5) + 55;

            if (dif <= 0)
                Life = MaximumLife;
            else
                Life = (MaximumLife - dif);

            Stats.PA.Bases = (Level >= 100 ? 7 : 6);
            Stats.PM.Bases = 3;

            Stats.dodgePA.Bases = 0;
            Stats.dodgePM.Bases = 0;
            Stats.prospection.Bases = 0;
            Stats.initiative.Bases = 0;
            Stats.maxPods.Bases = 1000;

            Stats.dodgePA.Bases = (Stats.wisdom.Bases / 4);
            Stats.dodgePM.Bases = (Stats.wisdom.Bases / 4);
            Stats.dodgePA.Items = (Stats.wisdom.Items / 4);
            Stats.dodgePM.Items = (Stats.wisdom.Items / 4);

            Stats.prospection.Bases = (Stats.luck.Total() / 10) + 100;

            if (Class == 3)
                Stats.prospection.Bases += 20;

            Stats.initiative.Bases = (MaximumLife / 4 + Stats.initiative.Total()) * (Life / MaximumLife);
        }

        public void ResetVita(string datas)
        {
            if (datas == "full")
            {
                Life = MaximumLife;
                SendChararacterStats();
            }
            else
            {
                Life = (MaximumLife / (int.Parse(datas) / 100));
                SendChararacterStats();
            }
        }

        public string SqlStats()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(CharactPoint).Append("|");
                builder.Append(SpellPoint).Append("|");
                builder.Append(Kamas).Append("|");
                builder.Append(Stats.life.Bases).Append("|");
                builder.Append(Stats.wisdom.Bases).Append("|");
                builder.Append(Stats.strenght.Bases).Append("|");
                builder.Append(Stats.intelligence.Bases).Append("|");
                builder.Append(Stats.luck.Bases).Append("|");
                builder.Append(Stats.agility.Bases);
            }

            return builder.ToString();
        }

        public void ParseStats(string args)
        {
            if (args == "") 
                return;

            var Data = args.Split('|');

            CharactPoint = int.Parse(Data[0]);
            SpellPoint = int.Parse(Data[1]);
            Kamas = long.Parse(Data[2]);
            Stats.life.Bases = int.Parse(Data[3]);
            Stats.wisdom.Bases = int.Parse(Data[4]);
            Stats.strenght.Bases = int.Parse(Data[5]);
            Stats.intelligence.Bases = int.Parse(Data[6]);
            Stats.luck.Bases = int.Parse(Data[7]);
            Stats.agility.Bases = int.Parse(Data[8]);
        }

        #endregion

        #region Params

        public string GetParam(string paramName)
        {
            switch (paramName)
            {
                case "kamas":
                    return Kamas.ToString();

                default:
                    return "";
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(Exp).Append(",");
                builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(Level).Character).Append(",");
                builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character).Append("|");
                builder.Append(Kamas).Append("|");
                builder.Append(CharactPoint).Append("|");
                builder.Append(SpellPoint).Append("|");
                builder.Append(Faction.ToString()).Append("|");
                builder.Append(Life).Append(",");
                builder.Append(MaximumLife).Append("|");
                builder.Append(Energy).Append(",10000|");
                builder.Append(Stats.initiative.Total()).Append("|");
                builder.Append(Stats.prospection.Total()).Append("|");

                builder.Append(Stats.PA.ToString()).Append("|");
                builder.Append(Stats.PM.ToString()).Append("|");
                builder.Append(Stats.strenght.ToString()).Append("|");
                builder.Append(Stats.life.ToString()).Append("|");
                builder.Append(Stats.wisdom.ToString()).Append("|");
                builder.Append(Stats.luck.ToString()).Append("|");
                builder.Append(Stats.agility.ToString()).Append("|");
                builder.Append(Stats.intelligence.ToString()).Append("|");
                builder.Append(Stats.PO.ToString()).Append("|");
                builder.Append(Stats.maxMonsters.ToString()).Append("|");
                builder.Append(Stats.bonusDamage.ToString()).Append("|");
                builder.Append(Stats.bonusDamagePhysic.ToString()).Append("|");
                builder.Append(Stats.bonusDamageMagic.ToString()).Append("|");
                builder.Append(Stats.bonusDamagePercent.ToString()).Append("|");
                builder.Append(Stats.bonusHeal.ToString()).Append("|");
                builder.Append(Stats.bonusDamageTrap.ToString()).Append("|");
                builder.Append(Stats.bonusDamageTrapPercent.ToString()).Append("|");
                builder.Append(Stats.returnDamage.ToString()).Append("|");
                builder.Append(Stats.bonusCritical.ToString()).Append("|");
                builder.Append(Stats.bonusFail.ToString()).Append("|");
                builder.Append(Stats.dodgePA.ToString()).Append("|");
                builder.Append(Stats.dodgePM.ToString()).Append("|");

                builder.Append(Stats.armorNeutral.ToString()).Append("|");
                builder.Append(Stats.armorPercentNeutral.ToString()).Append("|");
                builder.Append(Stats.armorPvpNeutral.ToString()).Append("|");
                builder.Append(Stats.armorPvpPercentNeutral.ToString()).Append("|");

                builder.Append(Stats.armorStrenght.ToString()).Append("|");
                builder.Append(Stats.armorPercentStrenght.ToString()).Append("|");
                builder.Append(Stats.armorPvpStrenght.ToString()).Append("|");
                builder.Append(Stats.armorPvpPercentNeutral.ToString()).Append("|");

                builder.Append(Stats.armorLuck.ToString()).Append("|");
                builder.Append(Stats.armorPercentLuck.ToString()).Append("|");
                builder.Append(Stats.armorPvpLuck.ToString()).Append("|");
                builder.Append(Stats.armorPvpPercentNeutral.ToString()).Append("|");

                builder.Append(Stats.armorAgility.ToString()).Append("|");
                builder.Append(Stats.armorPercentAgility.ToString()).Append("|");
                builder.Append(Stats.armorPvpAgility.ToString()).Append("|");
                builder.Append(Stats.armorPvpPercentNeutral.ToString()).Append("|");

                builder.Append(Stats.armorIntelligence.ToString()).Append("|");
                builder.Append(Stats.armorPercentIntelligence.ToString()).Append("|");
                builder.Append(Stats.armorPvpIntelligence.ToString()).Append("|");
                builder.Append(Stats.armorPvpPercentNeutral.ToString()).Append("|");

                builder.Append("1");
            }
            
            return builder.ToString();
        }

        #endregion
    }
}
