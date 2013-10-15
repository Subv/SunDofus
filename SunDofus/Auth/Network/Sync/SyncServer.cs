using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Auth.Network.Sync
{
    class SyncServer : Master.TCPServer
    {
        public List<SyncClient> Clients { get; set; }

        public SyncServer()
            : base(Utilities.Config.GetStringElement("SYNC_IP"), Utilities.Config.GetIntElement("SYNC_PORT"))
        {
            Clients = new List<SyncClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            Utilities.Loggers.Debug.Write(string.Format("New imputed sync connection <{0}> !", socket.IP));

            lock (Clients)
                Clients.Add(new SyncClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Loggers.Status.Write(string.Format("SyncServer starded on <{0}> !", remote));
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.Errors.Write(string.Format("SyncServer can't start : {0}", exception.ToString()));
        }
    }
}
