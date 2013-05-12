using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.NPC
{
    class NPCsQuestion
    {
        private int _questionID, _resQuesID;

        public int QuestionID
        {
            get
            {
                return _questionID;
            }
            set
            {
                _questionID = value;
            }
        }
        public int RescueQuestionID
        {
            get
            {
                return _resQuesID;
            }
            set
            {
                _resQuesID = value;
            }
        }

        public NPCsQuestion RescueQuestion;
        public List<NPCsAnswer> Answers;

        public List<Game.World.Conditions.NPCConditions> Conditions;

        public NPCsQuestion()
        {
            Answers = new List<NPCsAnswer>();
            Conditions = new List<Game.World.Conditions.NPCConditions>();
        }

        public bool HasConditions(Game.Characters.Character _character)
        {
            foreach (var condi in Conditions)
            {
                if (condi.HasCondition(_character))
                    continue;
                else
                    return false;
            }

            return true;
        }
    }
}