using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Auth.Network.Auth
{
    class AuthServer : Master.TCPServer
    {
        private List<AuthClient> _clients;

        public List<AuthClient> Clients
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
            : base(Utilities.Config.GetStringElement("Auth_Ip"), Utilities.Config.GetIntElement("Auth_Port"))
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

            Utilities.Loggers.InfosLogger.Write(string.Format("New imputed client connection <{0}> !", socket.IP));

            lock (Clients)
                Clients.Add(new AuthClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Loggers.StatusLogger.Write(string.Format("AuthServer starded on <{0}> !", remote));
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.ErrorsLogger.Write(string.Format("AuthServer can't start : {0}", exception.ToString()));
        }
    }
}
