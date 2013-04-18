using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network.Authentication
{
    class AuthenticationsKeys
    {
        public static List<AuthenticationsKeys> Keys = new List<AuthenticationsKeys>();

        private string _key;

        public string Key
        {
            get
            {
                return _key;
            }
        }
        public Entities.Models.Clients.AccountModel Infos;

        public AuthenticationsKeys(string packet)
        {
            Infos = new Entities.Models.Clients.AccountModel();
            var _datas = packet.Split('|');

            _key = _datas[1];
            Infos.ID = int.Parse(_datas[2]);
            Infos.Pseudo = _datas[3];
            Infos.Question = _datas[4];
            Infos.Answer = _datas[5];
            Infos.GMLevel = int.Parse(_datas[6]);
            Infos.Strcharacters = _datas[7];
            Infos.Subscription = long.Parse(_datas[8]);
            Infos.Strgifts = _datas[9];
        }
    }
}
