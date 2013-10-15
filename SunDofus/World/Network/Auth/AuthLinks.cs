using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using System.Timers;
using SunDofus.World.Entities.Models.Clients;

namespace SunDofus.World.Network.Auth
{
    class AuthLinks
    {
        private List<AuthClient> clients { get; set; }

        public AuthLinks()
        {
            clients = new List<AuthClient>();
        }

        public void Send(string message)
        {
            foreach (var client in clients)
                client.Send(message);
        }

        public void AddAuthClient(AuthClientModel model)
        {
            var client = new AuthClient(model);

            lock (clients)
                clients.Add(client);

            client.Start();
        }
    }
}
