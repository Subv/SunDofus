using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Exchanges
{
    class ExchangesManager
    {
        public static List<Exchange> Exchanges = new List<Exchange>();

        public static void AddExchange(Characters.Character traider, Characters.Character traided)
        {
            lock(Exchanges)
                Exchanges.Add(new Exchange(new ExchangePlayer(traider), new ExchangePlayer(traided)));

            traider.State.OnExchangePanel = true;
            traided.State.OnExchangePanel = true;

            traider.State.ActualPlayerExchange = traided.ID;
            traided.State.ActualPlayerExchange = traider.ID;

            traider.NetworkClient.Send("ECK1");
            traided.NetworkClient.Send("ECK1");
        }

        public static void LeaveExchange(Characters.Character canceler, bool must = true)
        {
            if (canceler.State.ActualNPC != -1)
            {
                canceler.State.ActualNPC = -1;
                canceler.State.OnExchange = false;
            }

            if (canceler.State.ActualTraided != -1)
            {
                if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == canceler.State.ActualTraided))
                {
                    var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == canceler.State.ActualTraided);

                    if (character.isConnected == true && must)
                        character.NetworkClient.Send("EV");

                    canceler.State.ActualTraided = -1;
                    canceler.State.ActualPlayerExchange = -1;
                    canceler.State.OnExchange = false;
                    canceler.State.OnExchangePanel = false;
                    canceler.State.OnExchangeAccepted = false;

                    character.State.ActualTraider = -1;
                    character.State.ActualPlayerExchange = -1;
                    character.State.OnExchange = false;
                    character.State.OnExchangePanel = false;
                    character.State.OnExchangeAccepted = false;

                    lock (Exchanges)
                    {
                        if (Exchanges.Any(x => (x.memberOne.Character == canceler && x.memberTwo.Character == character) || (x.memberTwo.Character == canceler && x.memberOne.Character == character)))
                            Exchanges.Remove(Exchanges.First(x => (x.memberOne.Character == canceler && x.memberTwo.Character == character) || (x.memberTwo.Character == canceler && x.memberOne.Character == character)));
                    }
                }
            }

            if (canceler.State.ActualTraider != -1)
            {
                if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == canceler.State.ActualTraider))
                {
                    var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == canceler.State.ActualTraider);

                    if (character.isConnected == true && must)
                        character.NetworkClient.Send("EV");

                    canceler.State.ActualTraider = -1;
                    canceler.State.ActualPlayerExchange = -1;
                    canceler.State.OnExchange = false;
                    canceler.State.OnExchangePanel = false;
                    canceler.State.OnExchangeAccepted = false;

                    character.State.ActualTraided = -1;
                    character.State.ActualPlayerExchange = -1;
                    character.State.OnExchange = false;
                    character.State.OnExchangePanel = false;
                    character.State.OnExchangeAccepted = false;

                    lock (Exchanges)
                    {
                        if (Exchanges.Any(x => (x.memberOne.Character == canceler && x.memberTwo.Character == character) || (x.memberTwo.Character == canceler && x.memberOne.Character == character)))
                            Exchanges.Remove(Exchanges.First(x => (x.memberOne.Character == canceler && x.memberTwo.Character == character) || (x.memberTwo.Character == canceler && x.memberOne.Character == character)));
                    }
                }
            }
        }
    }
}
