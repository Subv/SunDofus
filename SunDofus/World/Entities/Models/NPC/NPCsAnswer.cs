using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.NPC
{
    class NPCsAnswer
    {
        public int AnswerID { get; set; }
        public string Effects { get; set; }

        public List<Game.World.Conditions.NPCConditions> Conditions { get; set; }

        public NPCsAnswer()
        {
            Conditions = new List<Game.World.Conditions.NPCConditions>();
        }

        public void ApplyEffects(Game.Characters.Character character)
        {
            try
            {
                foreach (var effect in Effects.Split('|'))
                {
                    var infos = effect.Split(';');
                    Game.Effects.EffectAction.ParseEffect(character, int.Parse(infos[0]), infos[1]);
                }
            }
            catch { }
        }

        public bool HasConditions(Game.Characters.Character character)
        {
            foreach (var condi in Conditions)
            {
                if (condi.HasCondition(character))
                    continue;
                else
                    return false;
            }

            return true;
        }
    }
}
