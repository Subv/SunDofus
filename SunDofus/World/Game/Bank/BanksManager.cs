using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Bank
{
    class BanksManager
    {
        public static List<BankExchange> BanksExchanges = new List<BankExchange>();

        public static void OpenBank(Characters.Character character)
        {
            if (!Entities.Requests.BanksRequests.BanksList.Any(x => x.Owner == character.NetworkClient.Infos.ID))
            {
                var newBank = new Bank()
                {
                    Owner = character.NetworkClient.Infos.ID,
                    Kamas = 0,
                    isNewBank = true
                };

                Entities.Requests.BanksRequests.BanksList.Add(newBank);
            }

            var bank = Entities.Requests.BanksRequests.BanksList.First(x => x.Owner == character.NetworkClient.Infos.ID);
            var price = bank.Items.Count;

            if (price != 0 && price > character.Kamas)
            {
                character.NetworkClient.Send("Im182");
                return;
            }

            character.Kamas -= price;
            character.SendChararacterStats();

            BanksExchanges.Add(new BankExchange(bank, character));

            character.NetworkClient.Send("ECK5|");
            character.NetworkClient.Send(string.Format("EL{0};G{1}", bank.GetExchangeItems(), bank.Kamas));
            character.State.onExchangeWithBank = true;
        }

        public static void CloseBank(Characters.Character character)
        {
        }

        public static BankExchange FindExchange(Characters.Character character)
        {
            return BanksExchanges.First(x => x.Character == character && x.Bank.Owner == character.NetworkClient.Infos.ID);
        }
    }
}
