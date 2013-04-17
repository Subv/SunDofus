using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using System.Timers;
using SunDofus.World.Entities.Models.Clients;

namespace SunDofus.World.Network.Authentication
{
    class AuthenticationClient : Master.TCPClient
    {
        public AuthClientModel Model;

        private Timer _timer;
        private bool isLogged;
        private object _packetLocker;

        public AuthenticationClient(AuthClientModel model)
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
            Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot connect to @AuthServer@ because {0}", exception.ToString()));
        }

        private void DatasArrival(string datas)
        {
            lock (_packetLocker)
                ParsePacket(datas);
        }

        private void Disconnected()
        {
            Utilities.Loggers.StatusLogger.Write("Connection with the @AuthServer@ closed !");
            _timer.Start();
        }

        private void ParsePacket(string datas)
        {
            var infos = datas.Split('|');

            try
            {
                switch (infos[0])
                {
                    case "ANTS":

                        AuthenticationsKeys.m_keys.Add(new AuthenticationsKeys(datas));
                        break;

                    case "HCS":

                        Send(string.Format("SAI|{0}|{1}|{2}|{3}", Utilities.Config.GetIntElement("ServerId"),
                            Utilities.Config.GetStringElement("ServerIp"), Utilities.Config.GetIntElement("ServerPort"),
                            this.Model.PassKey), true);
                        break;

                    case "HCSS":

                        isLogged = true;
                        Utilities.Loggers.InfosLogger.Write("Connected with the @AuthenticationServer@ !");

                        if (ServersHandler.RealmServer.PseudoClients.Count > 0)
                            Send(string.Format("SNLC|{0}", string.Join("|", ServersHandler.RealmServer.PseudoClients.Values)));

                        break;
                }
            }
            catch (Exception e)
            {
                Utilities.Loggers.ErrorsLogger.Write(string.Format("Cannot parse @AuthServer's packet@ ({0}) because : {1}", datas, e.ToString()));
            }
        }
    }
}
