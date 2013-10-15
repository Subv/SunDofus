using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Network
{
    class ServersHandler
    {
        public static Realm.RealmServer RealmServer { get; set; }
        public static Auth.AuthLinks AuthLinks { get; set; }

        public static void InitialiseServers()
        {
            AuthLinks = new Auth.AuthLinks();

            RealmServer = new Realm.RealmServer();
            RealmServer.Start();
        }
    }
}
