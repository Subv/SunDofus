using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Auth.Entities.Models
{
    class ServersModel
    {
        private int _ID;
        private int _port;
        private int _state;
        private int opeID;

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
                opeID = _ID * 75;
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
        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
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

        private List<string> _clients;

        public List<string> GetClients
        {
            get 
            {
                return _clients;
            }
            set 
            {
                _clients = value;
            }
        }

        public ServersModel()
        {
            _clients = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};1", _ID, _state, opeID);
        }
    }
}
