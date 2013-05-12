using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Realm.Characters.Stats
{
    class AbstractStats
    {
        public int _bases, _items, _dons, _boosts;

        public int Bases
        {
            get
            {
                return _bases;
            }
            set                
            {
                _bases = value;
            }
        }
        public int Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }
        public int Dons
        {
            get
            {
                return _dons;
            }
            set
            {
                _dons = value;
            }
        }
        public int Boosts
        {
            get
            {
                return _boosts;
            }
            set
            {
                _boosts = value;
            }
        }

        public AbstractStats()
        {
            Bases = 0;
            Items = 0;
            Dons = 0;
            Boosts = 0;
        }

        public int Total()
        {
            return (Bases + Items + Dons + Boosts);
        }

        public int Base()
        {
            return Bases;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Bases, Items, Dons, Boosts);
        }
    }
}
