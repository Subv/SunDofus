using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Auth.Network.Auth
{
    class AuthServer : Master.TCPServer
    {
        public List<AuthClient> Clients { get; set; }

        public AuthServer()
            : base(Utilities.Config.GetStringElement("AUTH_IP"), Utilities.Config.GetIntElement("AUTH_PORT"))
        {
            Clients = new List<AuthClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);

            AuthQueue.Start();
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            Utilities.Loggers.Debug.Write(string.Format("New inputed client connection <{0}> !", socket.IP));

            lock (Clients)
                Clients.Add(new AuthClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Loggers.Status.Write(string.Format("AuthServer starded on <{0}> !", remote));
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.Errors.Write(string.Format("AuthServer can't start : {0}", exception.ToString()));
        }
    }
}
