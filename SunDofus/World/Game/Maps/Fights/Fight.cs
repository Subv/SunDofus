using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Maps.Fights
{
    class Fight
    {
        private FightType _type;
        private FightState _state;

        private int _ID;

        public int ID
        {
            get
            {
                return _ID;
            }
        }

        //private Characters.Character _actualPlayer;
        private List<Fighter> _fighters;

        public Fight(Characters.Character player1, Characters.Character player2, FightType type, Map map)
        {
            _fighters = new List<Fighter>();
            //_players = new List<Characters.Character>();
            //_players.Add(player1);
            //_players.Add(player2);

            _ID = map.NextFightID();
            _type = type;
            _state = FightState.Starting;
        }

        public void AddPlayer(Characters.Character player, int team)
        {
            //_players.Add(player);
        }

        public string BladesPattern
        {
            get
            {
                return string.Format("{0};{1}|{2};{3};{4};{5}|{6};{7};{8};{9}", _ID, (int)_state);
            }
        }

        public enum FightType
        {
            Challenge = 0,
            Agression = 1,
            PvMA = 2,
            MXvM = 3,
            PvM = 4,
            PvT = 5,
            PvMU = 6,
            Prisme = 7,
            Collector = 8,
        }

        public enum FightState
        {
            Starting,
            WaitTurn,
            None,
            Playing,
            Finished,
        }
    }
}
