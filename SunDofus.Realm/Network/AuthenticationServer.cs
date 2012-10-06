﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus;

namespace realm.Network
{
    class AuthenticationServer : AbstractServer
    {
        public List<Client.RealmClient> m_Clients;

        public AuthenticationServer()
            : base(Config.ConfigurationManager.GetString("Server_Ip"), Config.ConfigurationManager.GetInt("Server_Port"))
        {
            m_Clients = new List<Client.RealmClient>();
            this.RaiseAcceptEvent += new AcceptEvent(this.AcceptRealmClient);
            this.RaiseListenEvent += new OnListenEvent(this.ListenRealm);
            this.RaiseListenFailedEvent += new OnListenFailedEvent(this.ListenFailedRealm);
        }

        public void AcceptRealmClient(SilverSocket Socket)
        {
            if (Socket == null) return;
            SunDofus.Logger.Infos("New inputted connection !");
            m_Clients.Add(new Client.RealmClient(Socket));
        }

        public void ListenRealm(int m_Port)
        {
            SunDofus.Logger.Status("AuthenticationServer started on the port '" + m_Port + "' !");
        }

        public void ListenFailedRealm(Exception e)
        {
            SunDofus.Logger.Error(e);
        }
    }
}