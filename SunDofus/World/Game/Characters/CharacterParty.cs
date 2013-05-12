using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterParty
    {
        public Dictionary<Character, int> Members;

        private string _ownerName = "";
        private int _ownerID = -1;

        public CharacterParty(Character leader)
        {
            Members = new Dictionary<Character, int>();

            lock (Members)
                Members.Add(leader, 1);

            _ownerID = leader.ID;
            _ownerName = leader.Name;
        }

        public void AddMember(Character member)
        {
            lock(Members)
                Members.Add(member, 0);

            member.State.Party = this;

            if (Members.Count == 2)
            {
                Send(string.Format("PCK{0}", _ownerName));
                Send(string.Format("PL{0}", _ownerID));
                Send(string.Format("PM{0}", PartyPattern()));
            }
            else
            {
                member.NetworkClient.Send(string.Format("PCK{0}", _ownerName));
                member.NetworkClient.Send(string.Format("PL{0}", _ownerID));
                member.NetworkClient.Send(string.Format("PM{0}", PartyPattern()));

                foreach (var character in Members.Keys.ToList().Where(x => x != member).OrderByDescending(x => x.Stats.initiative.Total()))
                    character.NetworkClient.Send(string.Format("PM{0}", character.PatternOnParty()));
            }
        }

        public void LeaveParty(string name, string kicker = "")
        {
            if (!Members.Keys.ToList().Any(x => x.Name == name) || (kicker != "" || _ownerID != int.Parse(kicker)))
                return;

            var character = Members.Keys.ToList().First(x => x.Name == name);
            character.State.Party = null;

            lock (Members)
                Members.Remove(character);

            Send(string.Format("PM-{0}", character.ID));

            if (character.State.isFollow)
            {
                character.State.Followers.Clear();
                character.State.isFollow = false;
            }

            if (character.isConnected)
                character.NetworkClient.Send(string.Format("PV{0}", kicker));

            if (Members.Count == 1)
            {
                var last = Members.Keys.ToList()[0];
                last.State.Party = null;

                Members.Remove(last);

                if (last.isConnected)
                    last.NetworkClient.Send(string.Format("PV{0}", kicker));
            }
            else if (_ownerID == character.ID)
                GetNewLeader();
        }

        private void Send(string text)
        {
            foreach (var character in Members.Keys)
                character.NetworkClient.Send(text);
        }

        private void GetNewLeader()
        {
            var character = Members.Keys.ToList()[0];
            Members[character] = 1;

            _ownerID = character.ID;
            _ownerName = character.Name;

            Send(string.Format("PL{0}", _ownerID));
        }

        private string PartyPattern()
        {
            return string.Format("+{0}", string.Join("|", from x in Members.Keys.ToList() select x.PatternOnParty()));
        }
    }
}
