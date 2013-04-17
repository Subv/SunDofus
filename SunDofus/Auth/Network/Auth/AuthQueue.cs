using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SunDofus.Auth.Network.Auth
{
    class AuthQueue
    {
        private static List<AuthClient> _clients;

        public static List<AuthClient> Clients
        {
            get
            {
                return _clients;
            }
        }
        private static Timer _reloadTimer;

        public static void Start()
        {
            _clients = new List<AuthClient>();

            _reloadTimer = new Timer();
            {
                _reloadTimer.Interval = Utilities.Config.GetIntElement("Time_PerClient");
                _reloadTimer.Enabled = true;
                _reloadTimer.Elapsed += new ElapsedEventHandler(RefreshQueue);
                _reloadTimer.Start();
            }

            Utilities.Loggers.StatusLogger.Write("Queue for the servers' list started !");
        }

        public static void AddinQueue(AuthClient client)
        {
            Utilities.Loggers.InfosLogger.Write(string.Format("Add {0} in queue !", client.myIp()));

            lock(_clients)
                _clients.Add(client);
        }

        private static void RefreshQueue(object sender, EventArgs e)
        {
            if (_clients.Count <= 0)
                return;

            _clients[0].CheckAccount();

            lock (_clients)
                _clients.Remove(_clients[0]);

            Utilities.Loggers.InfosLogger.Write("Queue refreshed !");
        }
    }
}
