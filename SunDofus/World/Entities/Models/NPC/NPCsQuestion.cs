using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.NPC
{
    class NPCsQuestion
    {
        public int QuestionID { get; set; }
        public int RescueQuestionID { get; set; }

        public NPCsQuestion RescueQuestion { get; set; }
        public List<NPCsAnswer> Answers { get; set; }
        public List<string> Params { get; set; }

        public List<Game.World.Conditions.NPCConditions> Conditions { get; set; }

        public NPCsQuestion()
        {
            Answers = new List<NPCsAnswer>();
            Conditions = new List<Game.World.Conditions.NPCConditions>();
            Params = new List<string>();
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