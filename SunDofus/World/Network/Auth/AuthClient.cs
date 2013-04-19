using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using System.Timers;
using SunDofus.World.Entities.Models.Clients;

namespace SunDofus.World.Network.Auth
{
    class AuthClient : Master.TCPClient
    {
        public AuthClientModel Model;

        private Timer _timer;
        private bool isLogged;
        private object _packetLocker;

        public AuthClient(AuthClientModel model)
            : base(new SilverSocket())
        {
            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.DatasArrival);
            this.ConnectFailed += new ConnectFailedHandler(this.FailedToConnect);

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Enabled = true;
            _timer.Elapsed += new ElapsedEventHandler(this.TimeElapsed);

            _packetLocker = new object();
            isLogged = false;
            Model = model;
        }

        public void Start()
        {
            this.ConnectTo(Model.IP, Model.Port);
        }

        public void Send(string message, bool force = false)
        {
            if (isLogged == false && force == false)
                return;

            Utilities.Loggers.InfosLogger.Write(string.Format("Sent to {0} : {1}", myIp(), message));

            lock(_packetLocker)
                this.SendDatas(message);
        }

        private void TimeElapsed(object sender, EventArgs e)
        {
            if (this.Connected == false)
                Start();
            else
                _timer.Stop();
        }

        private void FailedToConnect(Exception exception)
        {
            Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot connect to AuthServer because {0}", exception.ToString()));
        }

        private void DatasArrival(string datas)
        {
            lock (_packetLocker)
                ParsePacket(datas);
        }

        private void Disconnected()
        {
            Utilities.Loggers.StatusLogger.Write("Connection with the AuthServer closed !");
            _timer.Start();
        }

        private void ParsePacket(string datas)
        {
            var infos = datas.Split('|');
            var nummer = Utilities.Basic.HexToDeci(infos[0]);

            try
            {
                switch (nummer)
                {
                    case 10:
                        ReceiveHelloConnect();
                        return;

                    case 30:
                        ReceiveHelloConnectSuccess();
                        return;

                    case 70:
                        ReceiveTranferTicket(datas.Substring(3));
                        return;
                }
            }
            catch (Exception e)
            {
                Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot parse AuthServer's packet ({0}) because : {1}", datas, e.ToString()));
            }
        }

        private void ReceiveHelloConnect()
        {
            var objects = new object[] { Utilities.Config.GetIntElement("ServerId"), Utilities.Config.GetStringElement("ServerIp"),
                            Utilities.Config.GetIntElement("ServerPort"), this.Model.PassKey };

            Send(new Packets.AuthenticationPacket().GetPacket(objects), true);
        }

        private void ReceiveHelloConnectSuccess()
        {
            isLogged = true;
            Utilities.Loggers.InfosLogger.Write("Connected with the AuthenticationServer !");

            if (ServersHandler.RealmServer.PseudoClients.Count > 0)
                Send(new Packets.ListOfConnectedPacket().GetPacket(ServersHandler.RealmServer.PseudoClients.Values));
        }

        private void ReceiveTranferTicket(string datas)
        {
            var infos = datas.Split('|');
            var key = infos[0];
            var pseudo = infos[2];
            var question = infos[3];
            var answer = infos[4];
            var characters = infos[6];
            var gifts = infos[8];

            int id;
            int level;
            long time;

            if (!int.TryParse(infos[1], out id))
                return;

            if (!int.TryParse(infos[5], out level))
                return;

            if (!long.TryParse(infos[7], out time))
                return;

            AuthKeys.Keys.Add(new AuthKeys.AuthKey
                (key, id, pseudo, question, answer, level, characters, time, gifts));
        }
    }
}
