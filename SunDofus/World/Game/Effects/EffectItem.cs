using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Effects
{
    class EffectItem
    {
        public int ID { get; set; }
        public int Value { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public string Effect { get; set; }

        public Characters.Character Client { get; set; }

        public EffectItem()
        {
            Value = 0;
            Value2 = 0;
            Value3 = 0;

            Effect = "0d0+0";
        }

        public EffectItem(EffectItem x)
        {
            ID = x.ID;

            Value = x.Value;
            Value2 = x.Value2;
            Value3 = x.Value3;

            Effect = x.Effect;
        }

        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}", Utilities.Basic.DeciToHex(ID), (Value <= 0 ? "" : Utilities.Basic.DeciToHex(Value)),
                (Value2 <= 0 ? "" : Utilities.Basic.DeciToHex(Value2)), (Value3 <= 0 ? "0" : Utilities.Basic.DeciToHex(Value3)), Effect);
        }

        public string SetString()
        {
            return string.Format("{0}#{1}#{2}", Utilities.Basic.DeciToHex(ID), (Value <= 0 ? "" : Utilities.Basic.DeciToHex(Value)),
                (Value2 <= 0 ? "" : Utilities.Basic.DeciToHex(Value2)));
        }

        public void ParseEffect(Characters.Character client)
        {
            Client = client;

            switch (ID)
            {
                case 110: // + Vie
                    AddLife();
                    break;

                case 111: // + PA
                    AddPA();
                    break;

                case 112: // + Dommage
                    AddDamage();
                    break;

                case 115: // + Coups critiques
                    AddCritic();
                    break;

                case 116: // - PO
                    SubPO();
                    break;

                case 117: // + PO
                    AddPO();
                    break;

                case 118: // + Force
                    AddStrenght();
                    break;

                case 119: // + Agilité
                    AddAgility();
                    break;

                case 122: // + Echec
                    AddFail();
                    break;

                case 123: // + Chance
                    AddLuck();
                    break;

                case 124: // + Wisdom
                    AddWisdom();
                    break;

                case 125: // + Vitalité
                    AddLife();
                    break;

                case 126: // + Intel
                    AddIntel();
                    break;

                case 128: // + PM
                    AddPM();
                    break;

                case 138: // + %Dommage
                    AddPercentDamage();
                    break;

                case 142: // + Dommage Physique
                    AddPhysicDamage();
                    break;

                case 143: // + Dommage Magique
                    AddMagicDamage();
                    break;

                case 152: // - Chance
                    SubLuck();
                    break;

                case 153: // - Vitalité
                    SubLife();
                    break;

                case 154: // - Agilité
                    SubAgility();
                    break;

                case 155: // - Intelligence
                    SubIntelligence();
                    break;

                case 156 : // - Sagesse
                    SubWisdom();
                    break;

                case 157: // - Force
                    SubStrenght();
                    break;

                case 158: // + Pods
                    AddPods();
                    break;

                case 159: // - Pods
                    SubPods();
                    break;

                case 160: // + Esquive PA
                    AddDodgePA();
                    break;

                case 161: // + Esquive PM
                    AddDodgePM();
                    break;

                case 162: // - Esquive PA
                    SubDodgePA();
                    break;

                case 163: // - Esquive PM
                    SubDodgePM();
                    break;
                    
                case 164: // - Dommage
                    SubDamage();
                    break;

                case 168: // - PA
                    SubPA();
                    break;

                case 169: // - PM
                    SubPM();
                    break;

                case 171: // - Coups critiques
                    SubCritic();
                    break;

                case 172: // - Dommage Magique
                    SubMagic();
                    break;

                case 173: // - Dommage Physique
                    SubPhysic();
                    break;

                case 174: // + Initiative
                    AddInitiative();
                    break;

                case 175: // - Initiative
                    SubInitiative();
                    break;

                case 176: // + Prospection
                    AddProspection();
                    break;

                case 177: // - Prospection
                    SubProspection();
                    break;

                case 178: // + Soins
                    AddHeal();
                    break;

                case 179: // - Soins
                    SubHeal();
                    break;

                case 182: // + Invocations
                    AddMonster();
                    break;

                case 210: // + %Armure terre
                    AddArmorStrenght();
                    break;

                case 211: // + %Armure Chance
                    AddArmorLuck();
                    break;

                case 212: // + %Armure Agilité
                    AddArmorAgility();
                    break;

                case 213: // + %Armure Intelligence
                    AddArmorIntelligence();
                    break;

                case 214: // + %Armure Neutre
                    AddArmorNeutral();
                    break;

                case 215: // - %Armure Terre
                    SubArmorPercentStrenght();
                    break;

                case 216: // -%Armure Chance
                    SubArmorPercentLuck();
                    break;

                case 217: // - %Armure Agilité
                    SubArmorPercentAgility();
                    break;

                case 218: // - %Armure Intel
                    SubArmorPercentIntelligence();
                    break;

                case 219: // - %Armure Neutre
                    SubArmorPercentNeutral();
                    break;

                case 225: // + Dommage Piege
                    AddTrapDamage();
                    break;

                case 226: // + %Dommage Piege
                    AddTrapPercentDamage();
                    break;

                case 240: // + Armor Force
                    AddArmorStrenght();
                    break;

                case 241: // + Armor Chance
                    AddArmorLuck();
                    break;

                case 242: // + Armor Agilité
                    AddArmorAgility();
                    break;

                case 243: // + Armor Intelligence
                    AddArmorIntelligence();
                    break;

                case 244: // + Armor Neutre
                    AddArmorNeutral();
                    break;

                case 245: // - Armor Force
                    SubArmorStrenght();
                    break;

                case 246: // - Armor Chance
                    SubArmorLuck();
                    break;

                case 247: // - Armor Agilité
                    SubArmorAgility();
                    break;

                case 248: // - Armor Intel
                    SubArmorIntelligence();
                    break;

                case 249: // - Armor Neutre
                    SubArmorNeutral();
                    break;

                case 250: // + %Pvp Armor Force
                    AddArmorPvpStrenghtPercent();
                    break;

                case 251: // + %Pvp Armor Chance
                    AddArmorPvpLuckPercent();
                    break;

                case 252: // + %Pvp Armor Agilité
                    AddArmorPvpAgilityPercent();
                    break;

                case 253: // + %Pvp Armor Intel
                    AddArmorPvpIntelligencePercent();
                    break;

                case 254: // + %Pvp Armor Neutre
                    AddArmorPvpNeutralPercent();
                    break;

                case 255: // -%Pvp Armor Force
                    SubArmorPvpStrenghtPercent();
                    break;

                case 256: // - %Pvp Armor Chance
                    SubArmorPvpLuckPercent();
                    break;

                case 257: // -%Pvp Armor Agilité
                    SubArmorPvpAgilityPercent();
                    break;

                case 258: // -%Pvp Armor Intel
                    SubArmorPvpIntelligencePercent();
                    break;

                case 259: // -%Pvp Armor Neutre
                    SubArmorPvpNeutralPercent();
                    break;

                case 260: // + Pvp Armor Force
                    AddArmorPvpStrenght();
                    break;

                case 261: // + Pvp Armor Chance
                    AddArmorPvpLuck();
                    break;

                case 262: // + Pvp Armor Agilité
                    AddArmorPvpAgility();
                    break;

                case 263: // + Pvp Armor Intelligence
                    AddArmorPvpIntelligence();
                    break;

                case 264: // + Pvp Armor Neutre
                    AddArmorPvpNeutral();
                    break;
            }
        }

        #region Effects' Void

        private void AddLife()
        {
            Client.Stats.life.Items += Value;
        }

        private void SubLife()
        {
            Client.Stats.life.Items -= Value;
        }

        private void AddPA()
        {
            Client.Stats.PA.Items += Value;
        }

        private void AddPM()
        {
            Client.Stats.PM.Items += Value;
        }

        private void AddPO()
        {
            Client.Stats.PO.Items += Value;
        }

        private void AddWisdom()
        {
            Client.Stats.wisdom.Items += Value;
        }

        private void AddStrenght()
        {
            Client.Stats.strenght.Items += Value;
        }

        private void AddIntel()
        {
            Client.Stats.intelligence.Items += Value;
        }

        private void AddLuck()
        {
            Client.Stats.luck.Items += Value;
        }

        private void AddAgility()
        {
            Client.Stats.agility.Items += Value;
        }

        private void AddInitiative()
        {
            Client.Stats.initiative.Items += Value;
        }

        private void AddProspection()
        {
            Client.Stats.prospection.Items += Value;
        }

        private void AddPercentDamage()
        {
            Client.Stats.bonusDamagePercent.Items += Value;
        }

        private void AddCritic()
        {
            Client.Stats.bonusCritical.Items += Value;
        }

        private void AddMagicDamage()
        {
            Client.Stats.bonusDamageMagic.Items += Value;
        }

        private void AddPhysicDamage()
        {
            Client.Stats.bonusDamagePhysic.Items += Value;
        }

        private void AddTrapDamage()
        {
            Client.Stats.bonusDamageTrap.Items += Value;
        }

        private void AddTrapPercentDamage()
        {
            Client.Stats.bonusDamageTrapPercent.Items += Value;
        }

        private void AddFail()
        {
            Client.Stats.bonusFail.Items += Value;
        }

        private void AddDodgePA()
        {
            Client.Stats.dodgePA.Items += Value;
        }

        private void AddDodgePM()
        {
            Client.Stats.dodgePM.Items += Value;
        }

        private void AddMonster()
        {
            Client.Stats.maxMonsters.Items += Value;
        }

        private void AddPods()
        {
            Client.Stats.maxPods.Items += Value;
        }

        private void AddHeal()
        {
            Client.Stats.bonusHeal.Items += Value;
        }

        private void AddDamage()
        {
            Client.Stats.bonusDamage.Items += Value;
        }

        private void SubAgility()
        {
            Client.Stats.agility.Items -= Value;
        }

        private void SubLuck()
        {
            Client.Stats.luck.Items -= Value;
        }

        private void SubDamage()
        {
            Client.Stats.bonusDamage.Items -= Value;
        }

        private void SubCritic()
        {
            Client.Stats.bonusCritical.Items -= Value;
        }

        private void SubMagic()
        {
            Client.Stats.bonusDamageMagic.Items -= Value;
        }

        private void SubPhysic()
        {
            Client.Stats.bonusDamagePhysic.Items -= Value;
        }

        private void SubDodgePA()
        {
            Client.Stats.dodgePA.Items -= Value;
        }

        private void SubDodgePM()
        {
            Client.Stats.dodgePM.Items -= Value;
        }

        private void SubStrenght()
        {
            Client.Stats.strenght.Items -= Value;
        }

        private void SubInitiative()
        {
            Client.Stats.initiative.Items -= Value;
        }

        private void SubIntelligence()
        {
            Client.Stats.intelligence.Items -= Value;
        }

        private void SubPA()
        {
            Client.Stats.PA.Items -= Value;
        }

        private void SubPM()
        {
            Client.Stats.PM.Items -= Value;
        }

        private void SubPO()
        {
            Client.Stats.PO.Items -= Value;
        }

        private void SubPods()
        {
            Client.Stats.maxPods.Items -= Value;
        }

        private void SubProspection()
        {
            Client.Stats.prospection.Items -= Value;
        }

        private void SubWisdom()
        {
            Client.Stats.wisdom.Items -= Value;
        }

        private void SubHeal()
        {
            Client.Stats.bonusHeal.Items -= Value;
        }

        private void SubVitality()
        {
            Client.Stats.life.Items -= Value;
        }

        private void AddArmorStrenght()
        {
            Client.Stats.armorStrenght.Items += Value;
        }

        private void AddArmorNeutral()
        {
            Client.Stats.armorNeutral.Items += Value;
        }

        private void AddArmorLuck()
        {
            Client.Stats.armorLuck.Items += Value;
        }

        private void AddArmorAgility()
        {
            Client.Stats.armorAgility.Items += Value;
        }

        private void AddArmorIntelligence()
        {
            Client.Stats.armorIntelligence.Items += Value;
        }

        private void AddArmorPercentStrenght()
        {
            Client.Stats.armorPercentStrenght.Items += Value;
        }

        private void AddArmorPercentIntelligence()
        {
            Client.Stats.armorPercentIntelligence.Items += Value;
        }

        private void AddArmorPercentAgility()
        {
            Client.Stats.armorPercentAgility.Items += Value;
        }

        private void AddArmorPercentLuck()
        {
            Client.Stats.armorPercentLuck.Items += Value;
        }

        private void AddArmorPercentNeutral()
        {
            Client.Stats.armorPercentNeutral.Items += Value;
        }

        private void AddArmorPvpStrenght()
        {
            Client.Stats.armorPvpStrenght.Items += Value;
        }

        private void AddArmorPvpNeutral()
        {
            Client.Stats.armorPvpNeutral.Items += Value;
        }

        private void AddArmorPvpLuck()
        {
            Client.Stats.armorPvpLuck.Items += Value;
        }

        private void AddArmorPvpAgility()
        {
            Client.Stats.armorPvpAgility.Items += Value;
        }

        private void AddArmorPvpIntelligence()
        {
            Client.Stats.armorPvpIntelligence.Items += Value;
        }

        private void AddArmorPvpStrenghtPercent()
        {
            Client.Stats.armorPvpPercentStrenght.Items += Value;
        }

        private void AddArmorPvpIntelligencePercent()
        {
            Client.Stats.armorPvpPercentIntelligence.Items += Value;
        }

        private void AddArmorPvpAgilityPercent()
        {
            Client.Stats.armorPvpPercentAgility.Items += Value;
        }

        private void AddArmorPvpLuckPercent()
        {
            Client.Stats.armorPvpPercentLuck.Items += Value;
        }

        private void AddArmorPvpNeutralPercent()
        {
            Client.Stats.armorPvpPercentNeutral.Items += Value;
        }

        private void SubArmorPercentStrenght()
        {
            Client.Stats.armorPercentStrenght.Items -= Value;
        }

        private void SubArmorPercentLuck()
        {
            Client.Stats.armorPercentLuck.Items -= Value;
        }

        private void SubArmorPercentAgility()
        {
            Client.Stats.armorPercentAgility.Items -= Value;
        }

        private void SubArmorPercentNeutral()
        {
            Client.Stats.armorPercentNeutral.Items -= Value;
        }

        private void SubArmorPercentIntelligence()
        {
            Client.Stats.armorPercentIntelligence.Items -= Value;
        }

        private void SubArmorStrenght()
        {
            Client.Stats.armorStrenght.Items -= Value;
        }

        private void SubArmorIntelligence()
        {
            Client.Stats.armorIntelligence.Items -= Value;
        }

        private void SubArmorAgility()
        {
            Client.Stats.armorAgility.Items -= Value;
        }

        private void SubArmorLuck()
        {
            Client.Stats.armorLuck.Items -= Value;
        }

        private void SubArmorNeutral()
        {
            Client.Stats.armorNeutral.Items -= Value;
        }

        private void SubArmorPvpStrenghtPercent()
        {
            Client.Stats.armorPvpPercentStrenght.Items -= Value;
        }

        private void SubArmorPvpNeutralPercent()
        {
            Client.Stats.armorPvpPercentNeutral.Items -= Value;
        }

        private void SubArmorPvpLuckPercent()
        {
            Client.Stats.armorPvpPercentLuck.Items -= Value;
        }

        private void SubArmorPvpAgilityPercent()
        {
            Client.Stats.armorPvpPercentAgility.Items -= Value;
        }

        private void SubArmorPvpIntelligencePercent()
        {
            Client.Stats.armorPvpPercentIntelligence.Items -= Value;
        }

        #endregion
    }
}
