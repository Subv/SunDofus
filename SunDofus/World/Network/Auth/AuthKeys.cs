using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Auth
{
    class AuthKeys
    {
        public static List<AuthKey> Keys = new List<AuthKey>();

        public class AuthKey
        {
            private string _key;

            public string Key
            {
                get
                {
                    return _key;
                }
            }
            public Entities.Models.Clients.AccountModel Infos;

            public AuthKey(string key, int id, string pseudo, string question, string answer, int level, string charac, long time, string gifts)
            {
                _key = key;

                Infos = new Entities.Models.Clients.AccountModel()
                {
                    ID = id,
                    Pseudo = pseudo,
                    Question = question,
                    Answer = answer,
                    GMLevel = level,
                    Strcharacters = charac,
                    Subscription = time,
                    Strgifts = gifts,
                };
            }
        }
    }
}
