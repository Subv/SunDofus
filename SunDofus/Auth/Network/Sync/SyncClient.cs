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
        private State state;
        private object locker;

        public ServersModel Server { get; set; }

        public SyncClient(SilverSocket socket) : base(socket)
        {
            state = State.OnAuthentication;
            locker = new object();

            this.ReceivedDatas += new ReceiveDatasHandler(this.PacketsReceived);
            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);

            Server = null;

            Send(new Packets.PHelloConnect().GetPacket());
        }

        public void SendTicket(string key, Auth.AuthClient client)
        {
            var datas = new object[] { key, client.Account.ID, client.Account.Pseudo, client.Account.Question,
                client.Account.Answer, client.Account.Level, string.Join(",", client.Account.Characters[Server.ID].ToArray()),
                client.Account.SubscriptionTime(), string.Join("+", Entities.Requests.GiftsRequests.GetGiftsByAccountID(client.Account.ID)),
                string.Join("+", client.Account.Friends), string.Join("+", client.Account.Enemies) };

            Send(new Packets.PTransfer().GetPacket(datas));
        }

        public void Send(string message)
        {
            Utilities.Loggers.Debug.Write(string.Format("Send to Sync [{0}] : {1}", IP, message));

            lock (locker)
                this.SendDatas(message);
        }

        private void PacketsReceived(string datas)
        {
            Utilities.Loggers.Debug.Write(string.Format("Received from sync [{0}] : {1}", IP, datas));

            lock (locker)
                Parse(datas);
        }

        private void Disconnected()
        {
            ChangeState(State.OnDisconnected);
            Utilities.Loggers.Debug.Write(string.Format("New closed sync connection <{0}> !", this.IP));

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
                        Authentication(datas);
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

                    case 130:
                        ReceiveToUpdateFriend(datas, true);
                        return;

                    case 140:
                        ReceiveToUpdateFriend(datas, false);
                        return;

                    case 150:
                        ReceiveToUpdateEnemy(datas, true);
                        return;

                    case 160:
                        ReceiveToUpdateEnemy(datas, false);
                        return;
                }
            }
            catch (Exception error)
            {
                Utilities.Loggers.Errors.Write(string.Format("Cannot parse sync packet : {0}", error.ToString()));
            }
        }

        private void Authentication(string[] datas)
        {
            var serverId = 0;
            var serverPort = 0;

            if (datas.Length < 4 || !int.TryParse(datas[1], out serverId) || !int.TryParse(datas[3], out serverPort))
            {
                Disconnect();
                return;
            }

            var serverIp = datas[2];
            var pass = datas[4];

            if (Entities.Requests.ServersRequests.Cache.Any(x => x.ID == serverId && x.IP == serverIp && x.Port == serverPort && x.State == 0))
            {
                var requieredServer = Entities.Requests.ServersRequests.Cache.First(x => x.ID == serverId && x.IP == serverIp && x.Port == serverPort && x.State == 0);

                if (!IP.Contains(serverIp) || pass != requieredServer.PassKey)
                {
                    Disconnect();
                    return;
                }

                Server = requieredServer;
                Send(new Packets.PHelloConnectSuccess().GetPacket());

                ChangeState(SyncClient.State.OnConnected, true);
                Utilities.Loggers.Debug.Write(string.Format("Sync <{0}> authentified !", this.IP));
            }
            else
                Disconnect();
        }

        private void ReceiveNewConnectedClient(string datas)
        {
            if (state == State.OnConnected)
                return;

            if (!Server.Clients.Contains(datas))
            {
                lock (Server.Clients)
                    Server.Clients.Add(datas);

                Entities.Requests.AccountsRequests.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(datas), true);
            }
        }

        private void ReceiveToUpdateFriend(string[] datas, bool add)
        {
            Entities.Requests.AccountsRequests.UpdateFriend(int.Parse(datas[1]), datas[2], add);
        }

        private void ReceiveToUpdateEnemy(string[] datas, bool add)
        {
            Entities.Requests.AccountsRequests.UpdateEnemy(int.Parse(datas[1]), datas[2], add);
        }

        private void ReceiveNewDisconnectedClient(string datas)
        {
            if (state != State.OnConnected)
                return;

            if (Server.Clients.Contains(datas))
            {
                lock (Server.Clients)
                    Server.Clients.Remove(datas);

                Entities.Requests.AccountsRequests.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(datas), false);
            }
        }

        private void ReceiveNewCreatedCharacter(string[] datas)
        {
            Entities.Requests.AccountsRequests.UpdateCharacters(int.Parse(datas[1]), datas[2], Server.ID);
        }

        private void ReceiveNewDeletedCharacter(string[] datas)
        {
            Entities.Requests.AccountsRequests.UpdateCharacters(int.Parse(datas[1]), datas[2], Server.ID, false);
        }

        private void ReceiveNewDeletedGifts(string[] datas)
        {
            Entities.Requests.GiftsRequests.DeleteGift(int.Parse(datas[1]), int.Parse(datas[2]));
        }

        private void ChangeState(State state, bool force = false)
        {
            if (Server == null || state == State.OnDisconnected && !force)
                return;

            this.state = state;

            switch (this.state)
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
            if (state != State.OnConnected)
                return;

            var packet = _datas.Substring(3).Split('|');

            foreach (var pseudo in packet)
            {
                if (!Server.Clients.Contains(pseudo))
                {
                    lock(Server.Clients)
                        Server.Clients.Add(pseudo);

                    Entities.Requests.AccountsRequests.UpdateConnectedValue(Entities.Requests.AccountsRequests.GetAccountID(pseudo), true);
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
