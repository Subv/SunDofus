using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Levels
{
    class LevelModel
    {
        private int _ID;
        private long _character, _job, _alignment, _guild, _mount;

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public long Character
        {
            get
            {
                return _character;
            }
            set
            {
                _character = value;
            }
        }
        public long Job
        {
            get
            {
                return _job;
            }
            set
            {
                _job = value;
            }
        }
        public long Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                _alignment = value;
            }
        }
        public long Guild
        {
            get
            {
                return _guild;
            }
            set
            {
                _guild = value;
            }
        }
        public long Mount
        {
            get
            {
                return _mount;
            }
            set
            {
                _mount = value;
            }
        }

        public LevelModel(long _max = 0)
        {
            Character = _max;
            Job = _max;
            Mount = _max;
            Alignment = _max;
            Guild = _max;
        }
    }
}
