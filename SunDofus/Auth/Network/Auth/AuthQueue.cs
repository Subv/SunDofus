﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SunDofus.Auth.Network.Auth
{
    class AuthQueue
    {
        public static List<AuthClient> Clients { get; set; }
        public static long LastAction { get; set; }

        private static Timer timer;
        private static bool isRunning;

        public static void Start()
        {
            Clients = new List<AuthClient>();

            timer = new Timer();
            {
                timer.Interval = Utilities.Config.GetIntElement("TIME_PERCLIENT");
                timer.Enabled = true;
                timer.Elapsed += new ElapsedEventHandler(RefreshQueue);
                timer.Start();
            }

            isRunning = true;
            Utilities.Loggers.Status.Write("Realms' Queue started !");
        }

        public static void AddInQueue(AuthClient client)
        {
            Utilities.Loggers.Debug.Write(string.Format("Add {0} in queue !", client.IP));

            lock (Clients)
                Clients.Add(client);

            if (!isRunning)
            {
                isRunning = true;
                timer.Start();
            }
        }

        private static void RefreshQueue(object sender, EventArgs e)
        {
            if (Clients.Count <= 0)
                return;

            Clients[0].SendInformations();
            Clients[0].State = AuthClient.AccountState.OnServersList;

            lock (Clients)
                Clients.Remove(Clients[0]);

            if (Clients.Count <= 0)
            {
                isRunning = false;
                timer.Stop();
            }
        }
    }
}
