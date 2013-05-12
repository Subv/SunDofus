using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharactersManager
    {
        public static List<Character> CharactersList = new List<Character>();

        public static bool ExistsName(string name)
        {
            return CharactersList.Any(x => x.Name == name);
        }
    }
}
