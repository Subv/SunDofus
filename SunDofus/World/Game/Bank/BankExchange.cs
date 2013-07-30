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

        public static void MoveKamas(long length, bool add = true)
        {
        }

        public static void MoveItem(int itemID, int quantity, bool add = true)
        {
        }
    }
}
