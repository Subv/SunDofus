using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network
{
    class ServersHandler
    {
        public static Realm.RealmClientServer RealmServer;
        public static Auth.AuthLinks AuthLinks;

        public static void InitialiseServers()
        {
            AuthLinks = new Auth.AuthLinks();

            RealmServer = new Realm.RealmClientServer();
            RealmServer.Start();
        }
    }
}
