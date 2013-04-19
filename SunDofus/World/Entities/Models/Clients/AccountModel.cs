using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Clients
{
    class AccountModel
    {
        private int _ID;
        private int _level;

        private string _pseudo;
        private string _question;
        private string _answer;
        private string _strcharacters;
        private string _strgifts;

        private long _subscription;

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
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }
        public string Pseudo
        {
            get
            {
                return _pseudo;
            }
            set
            {
                _pseudo = value;
            }
        }
        public string Question
        {
            get
            {
                return _question;
            }
            set
            {
                _question = value;
            }
        }
        public string Answer
        {
            get
            {
                return _answer;
            }
            set
            {
                _answer = value;
            }
        }
        public string Strcharacters
        {
            get
            {
                return _strcharacters;
            }
            set
            {
                _strcharacters = value;
            }
        }
        public string Strgifts
        {
            get
            {
                return _strgifts;
            }
            set
            {
                _strgifts = value;
            }
        }
        public long Subscription
        {
            get
            {
                return _subscription;
            }
            set
            {
                _subscription = value;
            }
        }

        public List<string> Characters;
        public List<GiftModel> Gifts;

        public AccountModel()
        {
            Characters = new List<string>();
            Gifts = new List<GiftModel>();

            Pseudo = "";
            Question = "";
            Answer = "";
            ID = -1;
            Level = -1;
            Strcharacters = "";
            Subscription = 0;
            Strgifts = "";
        }

        public void ParseCharacters()
        {
            if (Strcharacters == "") 
                return;

            foreach (var datas in Strcharacters.Split(','))
            {
                lock (Characters)
                {
                    if (!Characters.Contains(datas))
                        Characters.Add(datas);
                }
            }
        }

        public void ParseGifts()
        {
            if (Strgifts == "") 
                return;

            var infos = Strgifts.Split('+');

            foreach (var datas in infos)
            {
                var giftDatas = datas.Split('~');
                var gift = new GiftModel();

                gift.ID = int.Parse(giftDatas[0]);
                gift.Title = giftDatas[1];
                gift.Message = giftDatas[2];
                gift.ItemID = int.Parse(giftDatas[3]);
                gift.Image = giftDatas[4];

                lock(Gifts)
                    Gifts.Add(gift);
            }
        }
    }
}
