using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Auth.Entities.Models
{
    class ServersModel
    {
        public int ID { get; set; }
        public int Port { get; set; }
        public int State { get; set; }

        public string IP { get; set; }
        public string PassKey { get; set; }

        public List<string> Clients { get; set; }

        public ServersModel()
        {
            Clients = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};1", ID, State, (ID * 75));
        }
    }
}
