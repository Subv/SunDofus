using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Realm.Characters
{
    class CharacterChannels
    {
        public List<Channel> Channels;
        public Character Client;

        public CharacterChannels(Character client)
        {
            Client = client;
            Channels = new List<Channel>();

            AddChannel('*', true);
            AddChannel('#', true);
            AddChannel('$', true);
            AddChannel('p', true);
            AddChannel('%', true);
            AddChannel('i', true);
            AddChannel(':', true);
            AddChannel('?', true);
            AddChannel('!', true);
            AddChannel('^', true);
        }

        public void AddChannel(char head, bool state)
        {
            lock (Channels)
                Channels.Add(new Channel(head, state));
        }

        public void SendChannels()
        {
            Client.NetworkClient.Send(string.Format("cC+{0}", string.Join("", from c in Channels select c.Head.ToString())));
        }

        public void ChangeChannelState(char head, bool state)
        {
            if (Channels.Any(x => x.Head == head))
            {
                Channels.First(x => x.Head == head).On = state;
                Client.NetworkClient.Send(string.Format("cC{0}{1}", (state ? "+" : "-"), head.ToString()));
            }
        }

        public class Channel
        {
            private char _head;
            private bool _on;

            public char Head
            {
                get
                {
                    return _head;
                }
            }
            public bool On
            {
                get
                {
                    return _on;
                }
                set
                {
                    _on = value;
                }
            }

            public Channel(char head, bool on)
            {
                _head = head;
                _on = on;
            }
        }
    }
}
