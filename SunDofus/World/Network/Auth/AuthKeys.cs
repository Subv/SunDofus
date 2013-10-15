using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Entities.Models.Clients;

namespace SunDofus.World.Network.Auth
{
    class AuthKeys
    {
        public static List<AuthKey> Keys = new List<AuthKey>();

        public class AuthKey
        {
            public string Key { get; set; }

            public AccountModel Infos { get; set; }

            public AuthKey(string key, int id, string pseudo, string question, string answer, int level, string charac, long time, string gifts, string friends, string enemies)
            {
                Key = key;

                Infos = new AccountModel()
                {
                    ID = id,
                    Pseudo = pseudo,
                    Question = question,
                    Answer = answer,
                    Level = level,
                    Strcharacters = charac,
                    Subscription = time,
                    Strgifts = gifts,
                    StrFriends = friends,
                    StrEnemies = enemies,
                };
            }
        }
    }
}
