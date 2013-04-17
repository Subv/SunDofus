using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using System.Timers;

namespace SunDofus.World.Network.Authentication
{
    class AuthenticationsLinks
    {
        private List<AuthenticationClient> Clients;

        public AuthenticationsLinks()
        {
            Clients = new List<AuthenticationClient>();
        }

        public void Send(string message)
        {
            foreach (var client in Clients)
                client.Send(message);
        }

        public void Start()
        {
            foreach (var client in Entities.Cache.AuthsCache.AuthsList)
                Clients.Add(new AuthenticationClient(client));

            foreach (var client in Clients)
                client.Start();
        }
    }
}
