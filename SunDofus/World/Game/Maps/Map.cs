using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps
{
    class Map
    {
        public List<Characters.Character> Characters;
        public List<Entities.Models.Maps.TriggerModel> Triggers;
        public List<Characters.NPC.NPCMap> Npcs;
        public List<Monsters.MonstersGroup> MonstersGroups;
        public List<Fights.Fight> Fights;
        public List<int> RushablesCells;

        public Entities.Models.Maps.MapModel Model;

        public Map(Entities.Models.Maps.MapModel map)
        {
            Model = map;

            RushablesCells = UncompressDatas();

            Characters = new List<Characters.Character>();
            Triggers = new List<Entities.Models.Maps.TriggerModel>();
            Npcs = new List<Characters.NPC.NPCMap>();
            MonstersGroups = new List<Monsters.MonstersGroup>();
            Fights = new List<Fights.Fight>();

            if (Model.Monsters.Count != 0 && RushablesCells.Count != 0)
                RefreshAllMonsters();
        }

        private void RefreshAllMonsters()
        {
            for (int i = 1; i <= Model.MaxMonstersGroup; i++)
                AddMonstersGroup();
        }

        public void AddMonstersGroup()
        {
            if (MonstersGroups.Count >= Model.MaxMonstersGroup)
                return;

            lock(MonstersGroups)
                MonstersGroups.Add(new Monsters.MonstersGroup(Model.Monsters, this));
        }

        public void Send(string message)
        {
            foreach (var character in Characters)
                character.NetworkClient.Send(message);
        }

        public void AddPlayer(Characters.Character character)
        {
            Send(string.Format("GM|+{0}", character.PatternDisplayChar()));

            character.NetworkClient.Send(string.Format("fC{0}", Fights.Count)); //Fight

            lock (Characters)
                Characters.Add(character);

            character.NetworkClient.Send(string.Format("GM{0}", CharactersPattern()));

            if(Npcs.Count > 0)
                character.NetworkClient.Send(string.Format("GM{0}", NPCsPattern()));

            if (MonstersGroups.Count > 0)
                character.NetworkClient.Send(string.Format("GM{0}", MonstersGroupsPattern()));
        }

        public void DelPlayer(Characters.Character character)
        {
            Send(string.Format("GM|-{0}", character.ID));

            lock(Characters)
                Characters.Remove(character);
        }

        public void AddFight(Fights.Fight fight)
        {
            /*
             *  Client.Send("Gc+" & Fight.BladesPattern)
                Client.Send("Gt" & Fight.TeamPattern(0))
                Client.Send("Gt" & Fight.TeamPattern(1))
             * */
        }

        public int NextFightID()
        {
            var i = 1;

            while(Fights.Any(x => x.ID == i))
                i++;

            return i;
        }

        public int NextNpcID()
        {
            var i = -1;

            while (Npcs.Any(x => x.ID == i) || MonstersGroups.Any(x => x.ID == i))
                i -= 1;

            return i;
        }

        private string CharactersPattern()
        {
            var packet = "";
            Characters.ForEach(x => packet += string.Format("|+{0}", x.PatternDisplayChar()));
            return packet;
        }

        private string NPCsPattern()
        {
            var packet = "";
            Npcs.ForEach(x => packet += string.Format("|+{0}", x.PatternOnMap()));
            return packet;
        }

        private string MonstersGroupsPattern()
        {
            var packet = "";
            MonstersGroups.ForEach(x => packet += string.Format("|+{0}", x.PatternOnMap()));
            return packet;
        }

        private List<int> UncompressDatas()
        {
            List<int> newList = new List<int>();

            for (int i = 1; i <= 479; i++)
                newList.Add(i);
            //Lecture in the MapData / Decrypt

            return newList;
        }

        private string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

        private int hashCodes(char a)
        {
            return hash.IndexOf(a);
        }

        private bool isValidCell(string datas)
        {
            var lengh = datas.Length - 1;
            var table = new int[5000];

            while (lengh >= 0)
            {
                table[lengh] = hashCodes(datas[lengh]);
                lengh -= 1;
            }

            return ((table[2] & 56) >> 3) != 0;
        }
    }
}
