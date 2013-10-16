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
        public AccountsModel Account { get; set; }
        public AccountState State { get; set; }

        private object locker;
        private string key;

        public AuthClient(SilverSocket socket) : base(socket)
        {
            locker = new object();
            key = Utilities.Basic.RandomString(32);

            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.PacketReceived);

            Send(string.Concat("HC", key));
        }

        public void SendInformations()
        {
            Send(string.Concat("Ad", Account.Pseudo));
            Send(string.Concat("Ac", Account.Communauty));

            RefreshHosts();

            Send(string.Concat("AlK", Account.Level));
            Send(string.Concat("AQ", Account.Question));
        }

        public void RefreshHosts()
        {
            var packet = string.Concat("AH",
                string.Join("|", Entities.Requests.ServersRequests.Cache));

            Send(packet);
        }

        public void Send(string message)
        {
            Utilities.Loggers.Debug.Write(string.Format("Send to [{0}] : {1}", this.IP, message));

            lock (locker)
                this.SendDatas(message);
        }

        private void PacketReceived(string datas)
        {
            Utilities.Loggers.Debug.Write(string.Format("Receive from client [{0}] : {1}", this.IP, datas));

            lock (locker)
                Parse(datas);
        }

        private void Disconnected()
        {
            Utilities.Loggers.Debug.Write(string.Format("New closed client connection <{0}> !", this.IP));

            lock (ServersHandler.AuthServer.Clients)
                ServersHandler.AuthServer.Clients.Remove(this);
        }

        private void Parse(string datas)
        {
            switch (State)
            {
                case AccountState.OnCheckingVersion:
                    ParseVersionPacket(datas);
                    return;

                case AccountState.OnCheckingAccount:
                    CheckAccount(datas);
                    return;

                case AccountState.OnCheckingQueue:
                    SendQueuePacket();
                    return;

                case AccountState.OnServersList:
                    ParseListPacket(datas);
                    return;
            }
        }

        private void SendQueuePacket()
        {
            Send(string.Format("Af{0}|{1}|0|2", (AuthQueue.Clients.IndexOf(this) + 2),
                (AuthQueue.Clients.Count > 2 ? AuthQueue.Clients.Count : 3)));
        }

        private void CheckAccount(string datas)
        {
            if (!datas.Contains("#1"))
                return;

            var infos = datas.Split('#');
            var username = infos[0];
            var password = infos[1];

            var requestedAccount = Entities.Requests.AccountsRequests.LoadAccount(username);

            if (requestedAccount != null && Utilities.Basic.Encrypt(requestedAccount.Password, key) == password)
            {
                Account = requestedAccount;

                Utilities.Loggers.Debug.Write(string.Format("Client <{0}> authentified !", Account.Pseudo));

                if (AuthQueue.Clients.Count >= Utilities.Config.GetIntElement("MAX_CLIENTS_INQUEUE"))
                {
                    Send("M00\0");
                    this.Disconnect();
                }
                else if ((Environment.TickCount - AuthQueue.LastAction) < 5000)
                {
                    State = AccountState.OnCheckingQueue;
                    AuthQueue.AddInQueue(this);
                    SendQueuePacket();
                }
                else
                {
                    SendInformations();
                    State = AccountState.OnServersList;
                }

                AuthQueue.LastAction = Environment.TickCount;
            }
            else
            {
                Send("AlEx");
                this.Disconnect();
            }
        }

        private void ParseVersionPacket(string datas)
        {
            if (datas.Contains(Utilities.Config.GetStringElement("LOGIN_VERSION")))
                State = AccountState.OnCheckingAccount;
            else
            {
                Send(string.Concat("AlEv", Utilities.Config.GetStringElement("LOGIN_VERSION")));
                this.Disconnect();
            }
        }

        private void ParseListPacket(string datas)
        {
            if (datas.Substring(0, 1) != "A")
                return;

            var packet = string.Empty;

            switch (datas[1])
            {
                case 'F':

                    if (Entities.Requests.ServersRequests.Cache.Any(x => x.Clients.Contains(datas.Substring(2))))
                    {
                        packet = string.Concat("AF", 
                            Entities.Requests.ServersRequests.Cache.First(x => x.Clients.Contains(datas.Substring(2))).ID);

                        Send(packet);
                    }

                    Send("AF");
                    return;

                case 'x':

                    packet = string.Concat("AxK", Account.SubscriptionTime());

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

                    if (!int.TryParse(datas.Substring(2), out id))
                        return;

                    if (!ServersHandler.SyncServer.Clients.Any(x => x.Server.ID == id))
                    {
                        Send("BN");
                        this.Disconnect();
                    }

                    var choosenServer = ServersHandler.SyncServer.Clients.First(x => x.Server.ID == id);
                    var key = Utilities.Basic.RandomString(16);

                    choosenServer.SendTicket(key, this);
                    packet = string.Format("AYK{0}:{1};{2}", choosenServer.Server.IP, choosenServer.Server.Port, key);

                    Send(packet);
                    return;
            }
        }

        public enum AccountState
        {
            OnCheckingVersion,
            OnCheckingAccount,
            OnCheckingQueue,
            OnServersList,
        }
    }
}
