using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Master
{
    class TCPClient
    {
        private SilverSocket socket;

        public bool Connected { get; set; }

        protected delegate void DisconnectedSocketHandler();
        protected DisconnectedSocketHandler DisconnectedSocket;

        private void OnDisconnectedSocket()
        {
            var evnt = DisconnectedSocket;
            if (evnt != null)
                evnt();
        }

        protected delegate void ReceiveDatasHandler(string message);
        protected ReceiveDatasHandler ReceivedDatas;

        private void OnReceivedDatas(string message)
        {
            var evnt = ReceivedDatas;
            if (evnt != null)
                evnt(message);
        }

        protected delegate void ConnectFailedHandler(Exception exception);
        protected ConnectFailedHandler ConnectFailed;

        private void OnConnectFailed(Exception exception)
        {
            var evnt = ConnectFailed;
            if (evnt != null)
                evnt(exception);
        }

        public TCPClient(SilverSocket nsocket)
        {
            socket = nsocket;

            socket.OnConnected += new SilverEvents.Connected(this.OnConnected);
            socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(this.OnDisconnected);
            socket.OnDataArrivalEvent += new SilverEvents.DataArrival(this.OnDatasArrival);
            socket.OnFailedToConnect += new SilverEvents.FailedToConnect(this.OnFailedToConnect);
        }

        #region Functions

        public void ConnectTo(string ip, int port)
        {
            socket.ConnectTo(ip, port);
        }

        public string IP
        {
            get
            {
                return socket.IP;
            }
        }

        public void Disconnect()
        {
            socket.CloseSocket();
        }

        protected void SendDatas(string message)
        {
            try
            {
                socket.Send(Encoding.UTF8.GetBytes(string.Concat(message, "\x00")));
            }
            catch { }
        }

        #endregion

        #region Events

        private void OnDatasArrival(byte[] datas)
        {
            foreach (var Packet in Encoding.UTF8.GetString(datas).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
                OnReceivedDatas(Packet);
        }

        private void OnConnected()
        {
            Connected = true;
        }

        private void OnFailedToConnect(Exception e)
        {
            OnConnectFailed(e);
        }

        private void OnDisconnected()
        {
            Connected = false;

            OnDisconnectedSocket();
        }

        #endregion
    }
}
