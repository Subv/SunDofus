using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Bank
{
    class BankExchange
    {
        public Bank Bank;
        public Characters.Character Character;

        public BankExchange(Bank bank, Characters.Character character)
        {
            Bank = bank;
            Character = character;
        }

        public void MoveKamas(long length, bool add = true)
        {
            if (add)
            {
                if (length > Character.Kamas)
                    length = Character.Kamas;
                else if (length < 0)
                    length = 0;

                Bank.Kamas += length;
                Character.Kamas -= length;

                Character.SendChararacterStats();
                Character.NetworkClient.Send(string.Format("EsKG{0}", Bank.Kamas));
            }
            else
            {
                if (length > Bank.Kamas)
                    length = Bank.Kamas;
                else if (length < 0)
                    length = 0;

                Bank.Kamas -= length;
                Character.Kamas += length;

                Character.SendChararacterStats();
                Character.NetworkClient.Send(string.Format("EsKG{0}", Bank.Kamas));
            }
        }

        public void MoveItem(int itemID, int quantity, bool add = true)
        {
        }
    }
}
