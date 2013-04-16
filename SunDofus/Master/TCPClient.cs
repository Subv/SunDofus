﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Master
{
    class TCPClient
    {
        private SilverSocket _socket;
        private bool isConnected = false;

        public bool Connected
        {
            get
            {
                return isConnected;
            }
        }

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

        public TCPClient(SilverSocket socket)
        {
            _socket = socket;

            _socket.OnConnected += new SilverEvents.Connected(this.OnConnected);
            _socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(this.OnDisconnected);
            _socket.OnDataArrivalEvent += new SilverEvents.DataArrival(this.OnDatasArrival);
            _socket.OnFailedToConnect += new SilverEvents.FailedToConnect(this.OnFailedToConnect);
        }

        #region Functions

        public void ConnectTo(string ip, int port)
        {
            _socket.ConnectTo(ip, port);
        }

        public string myIp()
        {
            return _socket.IP;
        }

        public void Disconnect()
        {
            _socket.CloseSocket();
        }

        protected void SendDatas(string message)
        {
            try
            {
                _socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", message)));
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
            isConnected = true;
        }

        private void OnFailedToConnect(Exception e)
        {
            OnConnectFailed(e);
        }

        private void OnDisconnected()
        {
            isConnected = false;

            OnDisconnectedSocket();
        }

        #endregion
    }
}
