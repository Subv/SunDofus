using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus.Auth.Entities.Models;

namespace SunDofus.Auth.Network.Auth
{
    class AuthClient : Master.TCPClient
    {
        public AccountsModel Account;

        private AccountState _state;

        private object _packetLocker;
        private string _actualInfos;
        private string _key;

        public AuthClient(SilverSocket socket) : base(socket)
        {
            _packetLocker = new object();
            _key = Utilities.Basic.RandomString(32);

            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.PacketReceived);

            Send(string.Format("HC{0}", _key));
        }

        public void Send(string message)
        {
            lock(_packetLocker)
                this.SendDatas(message);

            Utilities.Loggers.InfosLogger.Write(string.Format("Sent to <{0}> : {1}", myIp(), message));
        }

        public void SendInformations()
        {
            Send(string.Format("Ad{0}",Account.Pseudo));
            Send(string.Format("Ac{0}", Account.Communauty));

            RefreshHosts();

            Send(string.Format("AlK{0}", Account.Level));
            Send(string.Format("AQ{0}", Account.Question));
        }

        public void RefreshHosts()
        {
            var packet = string.Format("AH{0}",
                string.Join("|", Entities.Requests.ServersRequests.Cache));

            Send(packet);
        }

        public void CheckAccount()
        {
            if (!_actualInfos.Contains("#1"))
                return;

            var infos = _actualInfos.Split('#');
            var username = infos[0];
            var password = infos[1];

            var requestedAccount = Entities.Requests.AccountsRequests.LoadAccount(username);

            if (requestedAccount != null && Utilities.Basic.Encrypt(requestedAccount.Password, _key) == password)
            {
                Account = requestedAccount;

                Utilities.Loggers.InfosLogger.Write(string.Format("Client {0} authentified !", Account.Pseudo));
                _state = AccountState.OnServersList;

                SendInformations();
            }
            else
            {
                Send("AlEx");
                this.Disconnect();
            }
        }

        private void Disconnected()
        {
            Utilities.Loggers.InfosLogger.Write(string.Format("New closed client connection <{0}> !", this.myIp()));

            lock (ServersHandler.AuthServer.Clients)
                ServersHandler.AuthServer.Clients.Remove(this);
        }

        private void PacketReceived(string datas)
        {
            Utilities.Loggers.InfosLogger.Write(string.Format("Receive from client <{0}> : [{1}]", this.myIp(), datas));

            lock (_packetLocker)
                Parse(datas);
        }

        private void Parse(string datas)
        {
            if (datas.Contains("#1"))
            {
                _actualInfos = datas;
                return;
            }

            switch (_state)
            {
                case AccountState.OnCheckingVersion:
                    ParseVersionPacket(datas);
                    return;

                case AccountState.OnCheckingQueue:
                    Send(string.Format("Af{0}|{1}|0|2", (AuthQueue.Clients.IndexOf(this) + 1),
                        (AuthQueue.Clients.Count > 2 ? AuthQueue.Clients.Count : 3)));
                    return;

                case AccountState.OnServersList:
                    ParseListPacket(datas);
                    return;
            }
        }

        private void ParseVersionPacket(string datas)
        {
            if (datas.Contains(Utilities.Config.GetStringElement("Login_Version")))
            {
                if (AuthQueue.Clients.Count >= Utilities.Config.GetIntElement("Max_Clients_inQueue"))
                {
                    Send("M00\0");
                    this.Disconnect();
                }
                else
                {
                    _state = AccountState.OnCheckingQueue;
                    AuthQueue.AddinQueue(this);
                }
            }
            else
            {
                Send(string.Format("AlEv{0}", Utilities.Config.GetStringElement("Login_Version")));
                this.Disconnect();
            }
        }

        private void ParseListPacket(string initialPacket)
        {
            if (initialPacket.Substring(0, 1) != "A")
                return;

            var packet = string.Empty;

            switch (initialPacket[1])
            {
                case 'F':

                    if (Entities.Requests.ServersRequests.Cache.Any(x => x.GetClients.Contains(initialPacket.Substring(2))))
                    {
                        packet = string.Format("AF{0}", 
                            Entities.Requests.ServersRequests.Cache.First(x => x.GetClients.Contains(initialPacket.Substring(2))).ID);

                        Send(packet);
                    }
                    Send("AF");

                    return;

                case 'x':

                    packet = string.Format("AxK{0}", Account.SubscriptionTime());

                    foreach (var server in Entities.Requests.ServersRequests.Cache)
                    {
                        if (!Account.Characters.ContainsKey(server.ID))
                            Account.Characters.Add(server.ID, new List<string>());

                        packet = string.Format("{0}|{1},{2}", packet, server.ID, Account.Characters[server.ID].Count);
                    }

                    Send(packet);

                    return;

                case 'X':

                    var id = 0;

                    if (!int.TryParse(initialPacket.Substring(2), out id))
                        return;

                    if (ServersHandler.SyncServer.Clients.Any(x => x.Server.ID == id))
                    {
                        var server = ServersHandler.SyncServer.Clients.First(x => x.Server.ID == id);
                        var key = Utilities.Basic.RandomString(16);

                        server.SendTicket(key, this);

                        packet = string.Format("AYK{0}:{1};{2}", server.Server.IP, server.Server.Port, key);
                        Send(packet);
                    }
                    else
                    {
                        Send("BN");
                        this.Disconnect();
                    }

                    return;
            }
        }

        private enum AccountState
        {
            OnCheckingVersion,
            OnCheckingQueue,
            OnServersList,
        }
    }
}
