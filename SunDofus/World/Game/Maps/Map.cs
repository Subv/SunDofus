using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps
{
    class Map
    {
        public List<Characters.Character> Characters { get; set; }
        public List<Entities.Models.Maps.TriggerModel> Triggers { get; set; }
        public List<Characters.NPC.NPCMap> Npcs { get; set; }
        public List<Monsters.MonstersGroup> MonstersGroups { get; set; }
        public List<Fights.Fight> Fights { get; set; }
        public List<int> RushablesCells { get; set; }
        public Guilds.GuildCollector Collector { get; set; }

        public Entities.Models.Maps.MapModel Model { get; set; }

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
            Send(string.Concat("GM|+", character.PatternDisplayChar()));

            character.NetworkClient.Send(string.Concat("fC", Fights.Count)); //Fight

            lock (Characters)
                Characters.Add(character);

            if (Characters.Count > 0)
                character.NetworkClient.Send(string.Concat("GM", CharactersPattern()));

            if (Npcs.Count > 0)
                character.NetworkClient.Send(string.Concat("GM", NPCsPattern()));

            if (MonstersGroups.Count > 0)
                character.NetworkClient.Send(string.Concat("GM", MonstersGroupsPattern()));

            if (Collector != null && !Collector.IsInFight)
                character.NetworkClient.Send(string.Concat("GM", Collector.PatternMap()));
        }

        public void DelPlayer(Characters.Character character)
        {
            Send(string.Concat("GM|-", character.ID));

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
            return string.Join("|+", from c in Characters select c.PatternDisplayChar());
        }

        private string NPCsPattern()
        {
            return string.Join("|+", from n in Npcs select n.PatternOnMap());
        }

        private string MonstersGroupsPattern()
        {
            return string.Join("|+", from m in MonstersGroups select m.PatternOnMap());
        }

        #region mapdata

        private List<int> UncompressDatas()
        {
            List<int> newList = new List<int>();

            string data = DecypherData(Model.MapData, "");

            for (int i = 0; i < data.Length; i += 10)
            {
                string CurrentCell = data.Substring(i, 10);
                byte[] CellInfo = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i2 = CurrentCell.Length - 1; i2 >= 0; i2--)
                    CellInfo[i2] = (byte)hash.IndexOf(CurrentCell[i2]);

                var type = (CellInfo[2] & 56) >> 3;

                if (type != 0)
                    newList.Add(i / 10);
            }

            return newList;
        }

        public static string DecypherData(string Data, string DecryptKey)
        {
            try
            {
                string result = string.Empty;

                if (DecryptKey != "")
                {
                    DecryptKey = PrepareKey(DecryptKey);
                    int checkSum = CheckSum(DecryptKey) * 2;

                    for (int i = 0, k = 0; i < Data.Length; i += 2)
                        result += (char)(int.Parse(Data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber) ^ (int)(DecryptKey[(k++ + checkSum) % DecryptKey.Length]));

                    return Uri.UnescapeDataString(result);
                }
                else return Data;
            }
            catch { return ""; }
        }

        private static int CheckSum(string Data)
        {
            int result = 0;

            for (int i = 0; i < Data.Length; i++)
                result += Data[i] % 16;

            return result % 16;
        }

        private static string PrepareKey(string Key)
        {
            string keyResult = "";

            for (int i = 0; i < Key.Length; i += 2)
                keyResult += Convert.ToChar(int.Parse(Key.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));

            return Uri.UnescapeDataString(keyResult);
        }

        #endregion

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
