﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DofusOrigin.Realm.Characters.Spells
{
    class InventarySpells
    {
        public List<CharacterSpell> Spells;
        public Character Client;

        public InventarySpells(Character client)
        {
            Spells = new List<CharacterSpell>();
            Client = client;
        }

        public void ParseSpells(string datas)
        {
            var spells = datas.Split('|');

            foreach (var spell in spells)
            {
                var infos = spell.Split(',');
                AddSpells(int.Parse(infos[0]), int.Parse(infos[1]), int.Parse(infos[2]));
            }
        }

        public void LearnSpells()
        {
            foreach (var spell in Database.Cache.SpellsCache.SpellsToLearnList.Where(x => x.m_race == Client.Class && x.m_level <= Client.Level))
            {
                if (Spells.Any(x => x.ID == spell.m_spellID))
                    continue;

                AddSpells(spell.m_spellID, 1, spell.m_pos);
            }
        }

        public void AddSpells(int id, int level, int pos)
        {
            if (Spells.Any(x => x.ID == id)) 
                return;

            if (level < 1) level = 1;
            if (level > 6) level = 6;

            if (pos > 25) pos = 25;
            if (pos < 1) pos = 25;

            lock(Spells)
                Spells.Add(new CharacterSpell(id, level, pos));
        }

        public void SendAllSpells()
        {
            var packet = "";

            foreach (var spell in Spells)
                packet += string.Format("{0}~{1}~{2};", spell.ID, spell.Level, Maps.Pathfinding.GetDirChar(spell.Position));

            Client.NetworkClient.Send(string.Format("SL{0}", packet));
        }

        public string SaveSpells()
        {
            return string.Join("|", Spells);
        }
    }
}
