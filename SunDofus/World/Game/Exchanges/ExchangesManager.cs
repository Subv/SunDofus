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
                Exchanges.Add(new Exchange(traider, traided));

            traider.State.onExchangePanel = true;
            traided.State.onExchangePanel = true;

            traider.State.actualPlayerExchange = traided.ID;
            traided.State.actualPlayerExchange = traider.ID;

            traider.NetworkClient.Send("ECK1");
            traided.NetworkClient.Send("ECK1");
        }

        public static void LeaveExchange(Characters.Character canceler, bool must = true)
        {
            if (canceler.State.actualNPC != -1)
            {
                canceler.State.actualNPC = -1;
                canceler.State.onExchange = false;
            }

            if (canceler.State.actualTraided != -1)
            {
                if (SunDofus.World.Game.Characters.CharactersManager.CharactersList.Any(x => x.ID == canceler.State.actualTraided))
                {
                    var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == canceler.State.actualTraided);

                    if (character.isConnected == true && must)
                        character.NetworkClient.Send("EV");

                    canceler.State.actualTraided = -1;
                    canceler.State.actualPlayerExchange = -1;
                    canceler.State.onExchange = false;
                    canceler.State.onExchangePanel = false;
                    canceler.State.onExchangeAccepted = false;

                    character.State.actualTraider = -1;
                    character.State.actualPlayerExchange = -1;
                    character.State.onExchange = false;
                    character.State.onExchangePanel = false;
                    character.State.onExchangeAccepted = false;

                    lock (Exchanges)
                    {
                        if (Exchanges.Any(x => (x.player1 == canceler && x.player2 == character) || (x.player2 == canceler && x.player1 == character)))
                            Exchanges.Remove(Exchanges.First(x => (x.player1 == canceler && x.player2 == character) || (x.player2 == canceler && x.player1 == character)));
                    }
                }
            }

            if (canceler.State.actualTraider != -1)
            {
                if (SunDofus.World.Game.Characters.CharactersManager.CharactersList.Any(x => x.ID == canceler.State.actualTraider))
                {
                    var character = SunDofus.World.Game.Characters.CharactersManager.CharactersList.First(x => x.ID == canceler.State.actualTraider);

                    if (character.isConnected == true && must)
                        character.NetworkClient.Send("EV");

                    canceler.State.actualTraider = -1;
                    canceler.State.actualPlayerExchange = -1;
                    canceler.State.onExchange = false;
                    canceler.State.onExchangePanel = false;
                    canceler.State.onExchangeAccepted = false;

                    character.State.actualTraided = -1;
                    character.State.actualPlayerExchange = -1;
                    character.State.onExchange = false;
                    character.State.onExchangePanel = false;
                    character.State.onExchangeAccepted = false;

                    lock (Exchanges)
                    {
                        if (Exchanges.Any(x => (x.player1 == canceler && x.player2 == character) || (x.player2 == canceler && x.player1 == character)))
                            Exchanges.Remove(Exchanges.First(x => (x.player1 == canceler && x.player2 == character) || (x.player2 == canceler && x.player1 == character)));
                    }
                }
            }
        }
    }
}
