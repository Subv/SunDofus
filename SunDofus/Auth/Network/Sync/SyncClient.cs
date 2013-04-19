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
                        ParseListConnected(packet);
                        return;

                    case 50:
                        ChangeState(State.OnMaintenance);
                        return;

                    case 60:
                        ChangeState(State.OnConnected);
                        return;

                    case 80:
                        ReceiveNewConnectedClient(datas[1]);
                        return;

                    case 90:
                        ReceiveNewDisconnectedClient(datas[1]);
                        return;

                    case 100:
                        ReceiveNewCreatedCharacter(datas);
                        return;

                    case 110:
                        ReceiveNewDeletedCharacter(datas);
                        return;

                    case 120:
                        ReceiveNewDeletedGifts(datas);
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

                ChangeState(SyncClient.State.OnConnected, true);
                Utilities.Loggers.InfosLogger.Write(string.Format("Sync <{0}> authentified !", this.myIp()));
            }
            else
                Disconnect();
        }

        private void ReceiveNewConnectedClient(string datas)
        {
            if (_state == State.OnConnected)
                return;

            if (!Server.GetClients.Contains(datas))
            {
                lock (Server.GetClients)
                    Server.GetClients.Add(datas);

                SyncAction.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(datas), true);
            }
        }

        private void ReceiveNewDisconnectedClient(string datas)
        {
            if (_state != State.OnConnected)
                return;

            if (Server.GetClients.Contains(datas))
            {
                lock (Server.GetClients)
                    Server.GetClients.Remove(datas);

                SyncAction.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(datas), false);
            }
        }

        private void ReceiveNewCreatedCharacter(string[] datas)
        {
            SyncAction.UpdateCharacters(int.Parse(datas[1]), datas[2], Server.ID);
        }

        private void ReceiveNewDeletedCharacter(string[] datas)
        {
            SyncAction.UpdateCharacters(int.Parse(datas[1]), datas[2], Server.ID, false);
        }

        private void ReceiveNewDeletedGifts(string[] datas)
        {
            SyncAction.DeleteGift(int.Parse(datas[1]), int.Parse(datas[2]));
        }

        private void ChangeState(State state, bool force = false)
        {
            if (Server == null || state == State.OnDisconnected && !force)
                return;

            this._state = state;

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
            if (_state != State.OnConnected)
                return;

            var packet = _datas.Substring(3).Split('|');

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
