using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterEnemies
    {
        private Character character;

        public CharacterEnemies(Character _character)
        {
            character = _character;
        }

        public void SendEnemies()
        {
            var packet = "iL|";

            foreach (var enemy in character.NetworkClient.Enemies)
            {
                if (Network.ServersHandler.RealmServer.Clients.Any(x => x.Infos.Pseudo == enemy && x.Characters.Any(c => c.isConnected == true)))
                {
                    packet = string.Concat(packet, enemy);

                    var charact = Network.ServersHandler.RealmServer.Clients.First(x => x.Infos.Pseudo == enemy).Player;
                    bool seeLevel = (charact.NetworkClient.Friends.Contains(character.NetworkClient.Infos.Pseudo) ? true : false);

                    packet = string.Concat(packet, ";?;", charact.Name, ";", (seeLevel ? charact.Level.ToString() : "?"), ";", (seeLevel ? charact.Faction.ID.ToString() : "-1"),
                        ";", charact.Class.ToString(), ";", charact.Sex.ToString(), ";", charact.Skin.ToString(), "|");
                }
                else
                    packet = string.Concat(packet, enemy, "|");
            }

            character.NetworkClient.Send(packet.Substring(0, packet.Length - 1));
        }

        public void AddEnemy(string datas)
        {
            if (Network.ServersHandler.RealmServer.Clients.Any(x => x.Characters.Any(f => f.Name == datas)))
            {
                var charact = Network.ServersHandler.RealmServer.Clients.First(x => x.Characters.Any(f => f.Name == datas));

                if (!character.NetworkClient.Enemies.Contains(charact.Infos.Pseudo))
                {
                    character.NetworkClient.Enemies.Add(charact.Infos.Pseudo);
                    character.NetworkClient.Send(string.Concat("iAK", charact.Infos.Pseudo, ";2;", charact.Player.Name, ";36;10;0;100.FL."));

                    Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.CreatedEnemyPacket().GetPacket(character.NetworkClient.Infos.ID, charact.Infos.Pseudo));
                }
                character.NetworkClient.Send("iAEA");
            }
            else
                character.NetworkClient.Send("FDEf");
        }

        public void RemoveEnemy(string datas)
        {
            var name = datas.Substring(1);

            if (datas.Substring(0, 1) == "*")
            {
                if (character.NetworkClient.Enemies.Contains(name))
                {
                    character.NetworkClient.Enemies.Remove(name);
                    character.NetworkClient.Send("iDK");

                    Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedEnemyPacket().GetPacket(character.NetworkClient.Infos.ID, name));
                }
                else
                    character.NetworkClient.Send("FDEf");
            }
            else if (datas.Substring(0, 1) == "%")
            {
                if (Network.ServersHandler.RealmServer.Clients.Any(x => x.Characters.Any(f => f.Name == name)))
                {
                    var client = Network.ServersHandler.RealmServer.Clients.First(x => x.Characters.Any(f => f.Name == name));

                    if (character.NetworkClient.Enemies.Contains(client.Infos.Pseudo))
                    {
                        character.NetworkClient.Enemies.Remove(client.Infos.Pseudo);
                        character.NetworkClient.Send("iDK");

                        Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedEnemyPacket().GetPacket(character.NetworkClient.Infos.ID, client.Infos.Pseudo));
                    }
                    else
                        character.NetworkClient.Send("FDEf");
                }
                else
                    character.NetworkClient.Send("FDEf");
            }
        }
    }
}
