using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus.Auth.Entities.Models;

namespace SunDofus.Auth.Network.Sync
{
    class SyncClient : Master.TCPClient
    {
        public ServersModel Server;

        private State _state;
        private object _packetLocker;

        public SyncClient(SilverSocket socket) : base(socket)
        {
            _state = State.OnAuthentication;
            _packetLocker = new object();

            this.ReceivedDatas += new ReceiveDatasHandler(this.PacketsReceived);
            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);

            Server = null;

            Send(new Packets.HelloConnectPacket().GetPacket());
        }

        public void SendTicket(string key, Auth.AuthClient client)
        {
            var datas = new object[] { key, client.Account.ID, client.Account.Pseudo, client.Account.Question,
                client.Account.Answer, client.Account.Level, string.Join(",", client.Account.Characters[Server.ID].ToArray()),
                client.Account.SubscriptionTime(), string.Join("+", Entities.Requests.GiftsRequests.GetGiftsByAccountID(client.Account.ID)) };

            Send(new Packets.TransferPacket().GetPacket(datas));
        }

        public void Send(string message)
        {
            Utilities.Loggers.InfosLogger.Write(string.Format("Send to Sync [{0}] : {1}", myIp(), message));

            lock (_packetLocker)
                this.SendDatas(message);
        }

        private void PacketsReceived(string datas)
        {
            Utilities.Loggers.InfosLogger.Write(string.Format("Received from sync [{0}] : {1}", myIp(), datas));

            lock (_packetLocker)
                Parse(datas);
        }

        private void Disconnected()
        {
            ChangeState(State.OnDisconnected);
            Utilities.Loggers.InfosLogger.Write(string.Format("New closed sync connection <{0}> !", this.myIp()));

            lock (ServersHandler.SyncServer.Clients)
                ServersHandler.SyncServer.Clients.Remove(this);
        }

        private void Parse(string packet)
        {
            try
            {
                var datas = packet.Split('|');
                var nummer = Utilities.Basic.HexToDeci(datas[0]);

                switch (nummer)
                {
                    case 20:
                        Authentication(int.Parse(datas[1]), datas[2], int.Parse(datas[3]), datas[4]);
                        return;

                    case 40:
                        if (_state == State.OnConnected)
                            ParseListConnected(packet);
                        return;

                    case 50:
                        if (_state == State.OnConnected)
                            ChangeState(State.OnMaintenance);
                        return;

                    case 60:
                        if (_state == State.OnMaintenance)
                            ChangeState(State.OnConnected);
                        return;

                    case 80:
                        if (_state == State.OnConnected)
                            return;

                        if (!Server.GetClients.Contains(datas[1]))
                        {
                            lock (Server.GetClients)
                                Server.GetClients.Add(datas[1]);

                            SyncAction.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(datas[1]), true);
                        }
                        return;

                    case 90:
                        if (_state != State.OnConnected)
                            return;

                        if (Server.GetClients.Contains(datas[1]))
                        {
                            lock (Server.GetClients)
                                Server.GetClients.Remove(datas[1]);

                            SyncAction.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(datas[1]), false);
                        }
                        return;

                    case 100:
                        if (_state == State.OnConnected)
                            SyncAction.UpdateCharacters(int.Parse(datas[1]), datas[2], Server.ID);
                        return;

                    case 110:
                        if (_state == State.OnConnected)
                            SyncAction.UpdateCharacters(int.Parse(datas[1]), datas[2], Server.ID, false);
                        return;

                    case 120:
                        if (_state == State.OnConnected)
                            SyncAction.DeleteGift(int.Parse(datas[1]), int.Parse(datas[2]));
                        return;
                }
            }
            catch (Exception e)
            {
                Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot parse sync packet : {0}", e.ToString()));
            }
        }

        private void Authentication(int serverId, string serverIp, int serverPort, string pass)
        {
            if (Entities.Requests.ServersRequests.Cache.Any(x => x.ID == serverId && x.IP == serverIp && x.Port == serverPort && x.State == 0))
            {
                var requieredServer = Entities.Requests.ServersRequests.Cache.First(x => x.ID == serverId && x.IP == serverIp && x.Port == serverPort && x.State == 0);

                if (!myIp().Contains(serverIp) || pass != requieredServer.PassKey)
                {
                    Disconnect();
                    return;
                }

                Server = requieredServer;
                Send(new Packets.HelloConnectSuccessPacket().GetPacket());

                ChangeState(SyncClient.State.OnConnected);
                Utilities.Loggers.InfosLogger.Write(string.Format("Sync <{0}> authentified !", this.myIp()));
            }
            else
                Disconnect();
        }

        private void ChangeState(State state)
        {
            this._state = state;

            if (Server == null) 
                return;

            switch (this._state)
            {
                case State.OnAuthentication:
                    Server.State = 0;
                    break;

                case State.OnConnected:
                    Server.State = 1;
                    break;

                case State.OnDisconnected:
                    Server.State = 0;
                    break;

                case State.OnMaintenance:
                    Server.State = 2;
                    break;
            }

            ServersHandler.AuthServer.Clients.Where(x => x.State
                == SunDofus.Auth.Network.Auth.AuthClient.AccountState.OnServersList).ToList().ForEach(x => x.RefreshHosts());
        }

        private void ParseListConnected(string _datas)
        {
            var packet = _datas.Substring(5).Split('|');

            foreach (var pseudo in packet)
            {
                if (!Server.GetClients.Contains(pseudo))
                {
                    lock(Server.GetClients)
                        Server.GetClients.Add(pseudo);

                    SyncAction.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(pseudo), true);
                }
            }
        }

        private enum State
        {
            OnAuthentication,
            OnConnected,
            OnDisconnected,
            OnMaintenance,
        }
    }
}
