using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Clients
{
    class AuthClientModel
    {
        public int ID { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }
        public string PassKey { get; set; }
    }
}
