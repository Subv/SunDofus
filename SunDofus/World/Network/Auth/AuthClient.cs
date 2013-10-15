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
        public AuthClientModel Model { get; set; }

        private Timer timer { get; set; }
        private bool isLogged { get; set; }
        private object locker { get; set; }

        public AuthClient(AuthClientModel model)
            : base(new SilverSocket())
        {
            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.DatasArrival);
            this.ConnectFailed += new ConnectFailedHandler(this.FailedToConnect);

            timer = new Timer();
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(this.TimeElapsed);

            locker = new object();
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

            Utilities.Loggers.Debug.Write(string.Format("Sent to {0} : {1}", IP, message));

            lock(locker)
                this.SendDatas(message);
        }

        private void TimeElapsed(object sender, EventArgs e)
        {
            if (this.Connected == false)
                Start();
            else
                timer.Stop();
        }

        private void FailedToConnect(Exception exception)
        {
            Utilities.Loggers.Errors.Write(string.Concat("Cannot connect to AuthServer because ", exception.ToString()));
        }

        private void DatasArrival(string datas)
        {
            lock (locker)
                ParsePacket(datas);
        }

        private void Disconnected()
        {
            Utilities.Loggers.Status.Write("Connection with the AuthServer closed !");
            timer.Start();
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
                Utilities.Loggers.Errors.Write(string.Format("Cannot parse AuthServer's packet ({0}) because : {1}", datas, e.ToString()));
            }
        }

        private void ReceiveHelloConnect()
        {
            var objects = new object[] { Utilities.Config.GetIntElement("SERVERID"), Utilities.Config.GetStringElement("SERVERIP"),
                            Utilities.Config.GetIntElement("SERVERPORT"), this.Model.PassKey };

            Send(new Packets.AuthenticationPacket().GetPacket(objects), true);
        }

        private void ReceiveHelloConnectSuccess()
        {
            isLogged = true;
            Utilities.Loggers.Debug.Write("Connected with the AuthenticationServer !");

            if (ServersHandler.RealmServer.PseudoClients.Count > 0)
                Send(new Packets.ListOfConnectedPacket().GetPacket(ServersHandler.RealmServer.PseudoClients.Values));
        }

        private void ReceiveTranferTicket(string datas)
        {
            var infos = datas.Split('|');

            if (infos.Length < 11)
                return;

            var key = infos[0];
            var pseudo = infos[2];
            var question = infos[3];
            var answer = infos[4];
            var characters = infos[6];
            var gifts = infos[8];
            var friends = infos[9];
            var enemies = infos[10];

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
                (key, id, pseudo, question, answer, level, characters, time, gifts, friends, enemies));
        }
    }
}
