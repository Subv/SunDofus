using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using System.Timers;

namespace SunDofus.World.Network.Auth
{
    class AuthLinks
    {
        private List<AuthClient> _clients;

        public AuthLinks()
        {
            _clients = new List<AuthClient>();
        }

        public void Send(string message)
        {
            foreach (var client in _clients)
                client.Send(message);
        }

        public void AddAuthClient(Entities.Models.Clients.AuthClientModel model)
        {
            var client = new AuthClient(model);

            lock (_clients)
                _clients.Add(client);
            client.Start();
        }
    }
}
