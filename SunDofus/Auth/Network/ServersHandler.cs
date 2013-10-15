using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Auth.Network.Auth;
using SunDofus.Auth.Network.Sync;

namespace SunDofus.Auth.Network
{
    class ServersHandler
    {
        public static AuthServer AuthServer { get; set; }
        public static SyncServer SyncServer { get; set; }

        public static void InitialiseServers()
        {
            AuthServer = new AuthServer();
            AuthServer.Start();

            SyncServer = new SyncServer();
            SyncServer.Start();
        }
    }
}
