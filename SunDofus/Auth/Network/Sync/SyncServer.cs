using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Auth.Network.Sync
{
    class SyncServer : Master.TCPServer
    {
        private List<SyncClient> _clients;

        public List<SyncClient> Clients
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

        public SyncServer()
            : base(Utilities.Config.GetStringElement("Sync_Ip"), Utilities.Config.GetIntElement("Sync_Port"))
        {
            _clients = new List<SyncClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            Utilities.Loggers.InfosLogger.Write(string.Format("New imputed sync connection <{0}> !", socket.IP));

            lock (Clients)
                Clients.Add(new SyncClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Loggers.StatusLogger.Write(string.Format("SyncServer starded on <{0}> !", remote));
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.ErrorsLogger.Write(string.Format("SyncServer can't start : {0}", exception.ToString()));
        }
    }
}
