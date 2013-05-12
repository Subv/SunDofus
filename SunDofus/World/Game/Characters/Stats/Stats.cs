using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters.Stats
{
    class Stats
    {
        public AbstractStats life = new AbstractStats();
        public AbstractStats wisdom = new AbstractStats();
        public AbstractStats strenght = new AbstractStats();
        public AbstractStats intelligence = new AbstractStats();
        public AbstractStats luck = new AbstractStats();
        public AbstractStats agility = new AbstractStats();

        public AbstractStats PA = new AbstractStats();
        public AbstractStats PM = new AbstractStats();
        public AbstractStats PO = new AbstractStats();

        public AbstractStats maxPods = new AbstractStats();

        public AbstractStats maxMonsters = new AbstractStats();

        public AbstractStats bonusDamage = new AbstractStats();
        public AbstractStats bonusDamagePhysic = new AbstractStats();
        public AbstractStats bonusDamageMagic = new AbstractStats();
        public AbstractStats bonusDamagePercent = new AbstractStats();
        public AbstractStats bonusHeal = new AbstractStats();
        public AbstractStats bonusDamageTrap = new AbstractStats();
        public AbstractStats bonusDamageTrapPercent = new AbstractStats();
        public AbstractStats bonusCritical = new AbstractStats();
        public AbstractStats bonusFail = new AbstractStats();

        public AbstractStats returnDamage = new AbstractStats();

        public AbstractStats initiative = new AbstractStats();
        public AbstractStats prospection = new AbstractStats();
        public AbstractStats dodgePA = new AbstractStats();
        public AbstractStats dodgePM = new AbstractStats();

        public AbstractStats armorNeutral = new AbstractStats();
        public AbstractStats armorPercentNeutral = new AbstractStats();
        public AbstractStats armorPvpNeutral = new AbstractStats();
        public AbstractStats armorPvpPercentNeutral = new AbstractStats();

        public AbstractStats armorStrenght = new AbstractStats();
        public AbstractStats armorPercentStrenght = new AbstractStats();
        public AbstractStats armorPvpStrenght = new AbstractStats();
        public AbstractStats armorPvpPercentStrenght = new AbstractStats();

        public AbstractStats armorLuck = new AbstractStats();
        public AbstractStats armorPercentLuck = new AbstractStats();
        public AbstractStats armorPvpLuck = new AbstractStats();
        public AbstractStats armorPvpPercentLuck = new AbstractStats();

        public AbstractStats armorAgility = new AbstractStats();
        public AbstractStats armorPercentAgility = new AbstractStats();
        public AbstractStats armorPvpAgility = new AbstractStats();
        public AbstractStats armorPvpPercentAgility = new AbstractStats();

        public AbstractStats armorIntelligence = new AbstractStats();
        public AbstractStats armorPercentIntelligence = new AbstractStats();
        public AbstractStats armorPvpIntelligence = new AbstractStats();
        public AbstractStats armorPvpPercentIntelligence = new AbstractStats();
    }
}
