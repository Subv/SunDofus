using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Guilds
{
    class Guild
    {
        private string _name;

        private int _ID;
        private int _bgID;
        private int _bgColor;
        private int _embID;
        private int _embColor;

        private int _level;
        private long _exp;
        private int _nbCollecMax;
        private int _collecProspection;
        private int _collecWisdom;
        private int _collecPods;
        private int _boostPoints;

        public bool isNewGuild = false;
        public bool mustDelete = false;

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
        public string Emblem
        {
            get
            {
                return string.Concat(Utilities.Basic.ToBase36(BgID), ",", Utilities.Basic.ToBase36(BgColor), ",",
                    Utilities.Basic.ToBase36(EmbID), ",", Utilities.Basic.ToBase36(EmbColor));
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
        public int BgID
        {
            get
            {
                return _bgID;
            }
            set
            {
                _bgID = value;
            }
        }
        public int BgColor
        {
            get
            {
                return _bgColor;
            }
            set
            {
                _bgColor = value;
            }
        }
        public int EmbID
        {
            get
            {
                return _embID;
            }
            set
            {
                _embID = value;
            }
        }
        public int EmbColor
        {
            get
            {
                return _embColor;
            }
            set
            {
                _embColor = value;
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
        public int CollectorMax
        {
            get
            {
                return _nbCollecMax;
            }
            set
            {
                _nbCollecMax = value;
            }
        }
        public int CollectorWisdom
        {
            get
            {
                return _collecWisdom;
            }
            set
            {
                _collecWisdom = value;
            }
        }
        public int CollectorProspection
        {
            get
            {
                return _collecProspection;
            }
            set
            {
                _collecProspection = value;
            }
        }
        public int CollectorPods
        {
            get
            {
                return _collecPods;
            }
            set
            {
                _collecPods = value;
            }
        }
        public int BoostPoints
        {
            get
            {
                return _boostPoints;
            }
            set
            {
                _boostPoints = value;
            }
        }

        public List<GuildMember> Members;
        public List<GuildCollector> Collectors;
        public Dictionary<int, int> Spells;

        public Guild()
        {
            Members = new List<GuildMember>();
            Collectors = new List<GuildCollector>();
            Spells = new Dictionary<int, int>();
        }

        public void AddMember(GuildMember member)
        {
            Members.Add(member);

            if (Members.Count < 2)
            {
                member.Rights = 29695;
                member.Rank = 1;
            }
            else
            {
                member.Rights = 0;
                member.Rank = 0;
            }
        }

        public void Send(string message)
        {
            Members.Where(x => x.Character.isConnected).ToList().ForEach(x => x.Character.NetworkClient.Send(message));
        }

        public string GetSpells()
        {
            var packet = string.Empty;

            foreach (var spell in Spells.Keys)
                packet = string.Concat(packet, spell, ";", Spells[spell], "|");

            return (packet != "" ? packet.Substring(0, packet.Length - 1) : packet);
        }

        public string GetSqlStats()
        {
            return string.Concat(CollectorMax, "~", CollectorPods, "~", CollectorProspection, "~", CollectorWisdom, "~", BoostPoints, "~", GetSpells());
        }

        public string GetSqlMembers()
        {
            return string.Join("|", from c in Members select c.SqlToString());
        }

        public string GetSqlEmblem()
        {
            return string.Concat(BgID, ";", BgColor, ";", EmbID, ";", EmbColor);
        }

        public void ParseSqlStats(string datas)
        {
            var infos = datas.Split('~');

            CollectorMax = int.Parse(infos[0]);
            CollectorPods = int.Parse(infos[1]);
            CollectorProspection = int.Parse(infos[2]);
            CollectorWisdom = int.Parse(infos[3]);
            BoostPoints = int.Parse(infos[4]);

            foreach (var spell in infos[5].Split('|'))
            {
                var spellinfos = spell.Split(';');
                Spells.Add(int.Parse(spellinfos[0]), int.Parse(spellinfos[1]));
            }
        }

        public void ParseSqlMembers(string datas)
        {
            foreach (var c in datas.Split('|'))
            {
                var memberInfos = c.Split(';');

                var character = Characters.CharactersManager.CharactersList.First(x => x.ID == int.Parse(memberInfos[0]));
                character.Guild = this;

                var member = new GuildMember(character);

                member.Rank = int.Parse(memberInfos[1]);
                member.ExpGaved = int.Parse(memberInfos[2]);
                member.ExpGived = int.Parse(memberInfos[3]);
                member.Rights = int.Parse(memberInfos[4]);

                Members.Add(member);
            }
        }

        public void ParseEmblem(string datas)
        {
            var infos = datas.Split(';');

            BgID = int.Parse(infos[0]);
            BgColor = int.Parse(infos[1]);
            EmbID = int.Parse(infos[2]);
            EmbColor = int.Parse(infos[3]);
        }
    }
}
