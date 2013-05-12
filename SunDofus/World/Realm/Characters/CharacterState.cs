using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Realm.Characters
{
    class CharacterState
    {
        private int _moveToCell = -1, _actNPC = -1, _actTraided = -1, _actTraider = -1, _actPlayEx = -1;
        private int _senderInvPar = -1, _recInvPar = -1;
        private int _followingID = -1;
        private int _onDiaWith = -1;
        private int _challAsked = -1, _challAsker = -1;

        Character Character;

        public CharacterState(Character character)
        {
            Character = character;
            created = false;

            Party = null;
            Followers = new List<Character>();
        }

        public bool created = false;

        public bool onMove = false;
        public bool onExchange = false;
        public bool onExchangePanel = false;
        public bool onExchangeAccepted = false;

        public int moveToCell
        {
            get
            {
                return _moveToCell;
            }
            set
            {
                _moveToCell = value;
            }
        }
        public int actualNPC
        {
            get
            {
                return _actNPC;
            }
            set
            {
                _actNPC = value;
            }
        }
        public int actualTraided
        {
            get
            {
                return _actTraided;
            }
            set
            {
                _actTraided = value;
            }
        }
        public int actualTraider
        {
            get
            {
                return _actTraider;
            }
            set
            {
                _actTraider = value;
            }
        }
        public int actualPlayerExchange
        {
            get
            {
                return _actPlayEx;
            }
            set
            {
                _actPlayEx = value;
            }
        }

        public bool onWaitingParty = false;
        public int senderInviteParty
        {
            get
            {
                return _senderInvPar;
            }
            set
            {
                _senderInvPar = value;
            }
        }
        public int receiverInviteParty
        {
            get
            {
                return _recInvPar;
            }
            set
            {
                _recInvPar = value;
            }
        }

        public bool isFollow = false;
        public bool isFollowing = false;
        public int followingID
        {
            get
            {
                return _followingID;
            }
            set
            {
                _followingID = value;
            }
        }

        public bool onDialoging = false;
        public int onDialogingWith
        {
            get
            {
                return _onDiaWith;
            }
            set
            {
                _onDiaWith = value;
            }
        }

        public bool isChallengeAsked = false;
        public bool isChallengeAsker = false;
        public int ChallengeAsked
        {
            get
            {
                return _challAsked;
            }
            set
            {
                _challAsked = value;
            }
        }
        public int ChallengeAsker
        {
            get
            {
                return _challAsker;
            }
            set
            {
                _challAsker = value;
            }
        }

        public CharacterParty Party;
        public List<Character> Followers;

        public bool Occuped
        {
            get
            {
                return (onMove || onExchange || onWaitingParty || onDialoging || isChallengeAsked || isChallengeAsker);
            }
        }
    }
}
