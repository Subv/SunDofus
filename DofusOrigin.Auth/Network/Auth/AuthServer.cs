﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace DofusOrigin.Network.Auth
{
    class AuthServer : DofusOrigin.Network.TCPServer
    {
        private List<AuthClient> _clients;

        public List<AuthClient> GetClients
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

        public AuthServer()
            : base(Utilities.Config.GetConfig.GetStringElement("Auth_Ip"), Utilities.Config.GetConfig.GetIntElement("Auth_Port"))
        {
            _clients = new List<AuthClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);

            AuthQueue.Start();
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            Utilities.Loggers.InfosLogger.Write(string.Format("New inputted realm connection @<{0}>@ !", socket.IP));

            lock (GetClients)
                GetClients.Add(new AuthClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Loggers.StatusLogger.Write(string.Format("@RealmServer@ starded on <{0}> !", remote));
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.ErrorsLogger.Write(string.Format("@RealmServer@ can't start : {0}", exception.ToString()));
        }
    }
}
