﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace auth.Database.Models
{
    class AccountModel
    {
        public int m_id { get; set; }
        public int m_level { get; set; }
        public int m_communauty { get; set; }

        public string m_username { get; set; }
        public string m_password { get; set; }
        public string m_pseudo { get; set; }
        public string m_question { get; set; }
        public string m_answer { get; set; }
        public string m_charstr { get; set; }

        public DateTime m_subscriptionDate;
        public Dictionary<int, List<string>> m_characters;

        public AccountModel()
        {
            m_subscriptionDate = new DateTime();
            m_characters = new Dictionary<int, List<string>>();
        }

        public void ParseCharacter(string _basestr)
        {
            if (_basestr == "")
            {
                m_characters = new Dictionary<int, List<string>>();
                return;
            }

            m_charstr = _basestr;
            var dico = new Dictionary<int, List<string>>();
            var datas = _basestr.Split(':');

            foreach (var infos in datas)
            {
                var characterdatas = infos.Split(',');

                if (!dico.ContainsKey(int.Parse(characterdatas[1]))) 
                    dico.Add(int.Parse(characterdatas[1]), new List<string>());

                dico[int.Parse(characterdatas[1])].Add(characterdatas[0]);
            }

            m_characters = dico;
        }

        public long GetSubscriptionTime()
        {
            var time = m_subscriptionDate.Subtract(DateTime.Now).TotalMilliseconds;

            if (Utilities.Config.m_config.GetBoolElement("Subscription_Time") == false)
                return 31536000000;
            else if (m_subscriptionDate.Subtract(DateTime.Now).TotalMilliseconds <= 1)
                return 0;
            else if (time >= Utilities.Config.m_config.GetLongElement("Max_Subscription_Time"))
                return 31536000000;

            return (long)time;
        }
    }
}
