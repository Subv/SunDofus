﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Timers;

namespace SunDofus.Entities
{
    class DatabaseProvider
    {
        public static MySqlConnection Connection;
        public static object ConnectionLocker;

        private static Timer _timer;
        private static bool _isConnected;
        private static int _lastAction;

        private static int _getLastActionTime
        {
            get
            {
                return (Environment.TickCount - _lastAction);
            }
        }

        public static void InitializeConnection()
        {
            _isConnected = false;

            Connection = new MySqlConnection(string.Format("server={0};uid={1};pwd='{2}';database={3}",
                    Utilities.Config.GetStringElement("Database_Server"),
                    Utilities.Config.GetStringElement("Database_User"),
                    Utilities.Config.GetStringElement("Database_Pass"),
                    Utilities.Config.GetStringElement("Database_Name")));

            ConnectionLocker = new object();

            lock (ConnectionLocker)
                Connection.Open();

            _isConnected = true;
            _lastAction = Environment.TickCount;

            Utilities.Loggers.StatusLogger.Write("Connected to the database !");

            _timer = new Timer();
            _timer.Interval = 10000;
            _timer.Elapsed += new ElapsedEventHandler(UpdateConnection);
            _timer.Start();

            Requests.AccountsRequests.ResetConnectedValue();
            Requests.ServersRequests.LoadCache();
        }

        public static void CheckConnection()
        {
            if (!_isConnected)
                ReConnect();

            _lastAction = Environment.TickCount;
        }

        private static void ReConnect()
        {
            lock (ConnectionLocker)
                Connection.Open();
            
            _isConnected = true;
            _timer.Start();

            Utilities.Loggers.StatusLogger.Write("Connected to the database !");
        }

        private static void UpdateConnection(object sender, EventArgs e)
        {
            if (_getLastActionTime >= 9000)
            {
                _isConnected = false;
                _timer.Stop();

                lock(ConnectionLocker)
                    Connection.Close();

                Utilities.Loggers.StatusLogger.Write("Disconnected from the database !");
            }
        }
    }
}
