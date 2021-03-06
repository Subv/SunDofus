﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus.World.Network;

namespace SunDofus.World.Network.Realm
{
    class RealmServer : Master.TCPServer
    {
        public List<RealmClient> Clients { get; set; }
        public Dictionary<string, int> PseudoClients { get; set; }

        public RealmServer()
            : base(Utilities.Config.GetStringElement("SERVERIP"), Utilities.Config.GetIntElement("SERVERPORT"))
        {
            Clients = new List<RealmClient>();
            PseudoClients = new Dictionary<string,int>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);

            RealmQueue.Start();
        }

        public void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            Utilities.Loggers.Debug.Write("New inputted client connection !");

            lock(Clients)
                Clients.Add(new RealmClient(socket));
        }

        public void OnListeningServer(string remote)
        {
            Utilities.Loggers.Status.Write(string.Format("RealmServer started on <{0}> !", remote));
        }

        public void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.Errors.Write(string.Concat("Cannot start the RealmServer because : ", exception.ToString()));
        }
    }
}
