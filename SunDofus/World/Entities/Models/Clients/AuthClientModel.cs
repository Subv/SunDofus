using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Clients
{
    class AuthClientModel
    {
        private int _ID;
        private int _port;
        private string _IP;
        private string _passKey;

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }
        public string IP
        {
            get
            {
                return _IP;
            }
            set
            {
                _IP = value;
            }
        }
        public string PassKey
        {
            get
            {
                return _passKey;
            }
            set
            {
                _passKey = value;
            }
        }
    }
}
