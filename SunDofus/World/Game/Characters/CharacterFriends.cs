using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterFriends
    {
        private Character character;

        public bool willSeeWhenConnected = false;

        public CharacterFriends(Character _character)
        {
            character = _character;
        }

        public void SendFriends()
        {
            var packet = "FL|";

            foreach (var friend in character.NetworkClient.Friends)
            {
                if (Network.ServersHandler.RealmServer.Clients.Any(x => x.Infos.Pseudo == friend && x.Characters.Any(c => c.isConnected == true)))
                {
                    packet = string.Concat(packet, friend);

                    var charact = Network.ServersHandler.RealmServer.Clients.First(x => x.Infos.Pseudo == friend).Player;
                    bool seeLevel = (charact.NetworkClient.Friends.Contains(character.NetworkClient.Infos.Pseudo) ? true : false);

                    packet = string.Concat(packet, ";?;", charact.Name, ";", (seeLevel ? charact.Level.ToString() : "?"), ";", (seeLevel ? charact.Faction.ID.ToString() : "-1"),
                        ";", charact.Class.ToString(), ";", charact.Sex.ToString(), ";", charact.Skin.ToString(), "|");
                }
                else
                    packet = string.Concat(packet, friend, "|");
            }

            character.NetworkClient.Send(packet.Substring(0, packet.Length - 1));
        }

        public void AddFriend(string datas)
        {
            if (Network.ServersHandler.RealmServer.Clients.Any(x => x.Characters.Any(f => f.Name == datas)))
            {
                var charact = Network.ServersHandler.RealmServer.Clients.First(x => x.Characters.Any(f => f.Name == datas));

                if (!character.NetworkClient.Friends.Contains(charact.Infos.Pseudo))
                {
                    character.NetworkClient.Friends.Add(charact.Infos.Pseudo);
                    bool seeLevel = (charact.Friends.Contains(character.NetworkClient.Infos.Pseudo) ? true : false);

                    var packet = string.Concat(charact.Infos.Pseudo, ";?;", charact.Player.Name, ";", (seeLevel ? charact.Player.Level.ToString() : "?"), ";", (seeLevel ? charact.Player.Faction.ID.ToString() : "-1"),
                        ";", charact.Player.Class.ToString(), ";", charact.Player.Sex.ToString(), ";", charact.Player.Skin.ToString(), "|");

                    character.NetworkClient.Send(string.Concat("FAK", packet));

                    //Send the information to AuthServer and Save
                }
                else
                    character.NetworkClient.Send("FAEa");
            }
            else
                character.NetworkClient.Send("FAEf");
        }

        public void RemoveFriend(string datas)
        {
            var name = datas.Substring(1);

            if (datas.Substring(0, 1) == "*")
            {
                if (character.NetworkClient.Friends.Contains(name))
                {
                    character.NetworkClient.Friends.Remove(name);
                    character.NetworkClient.Send("FDK");

                    //Send the information to AuthServer and Save
                }
                else
                    character.NetworkClient.Send("FDEf");
            }
            else if(datas.Substring(0,1) == "%")
            {
                if (Network.ServersHandler.RealmServer.Clients.Any(x => x.Characters.Any(f => f.Name == name)))
                {
                    var client = Network.ServersHandler.RealmServer.Clients.First(x => x.Characters.Any(f => f.Name == name));

                    if (character.NetworkClient.Friends.Contains(client.Infos.Pseudo))
                    {
                        character.NetworkClient.Friends.Remove(client.Infos.Pseudo);
                        character.NetworkClient.Send("FDK");

                        //Send the information to AuthServer and Save
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
