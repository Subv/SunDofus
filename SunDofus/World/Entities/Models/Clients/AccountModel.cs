using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Clients
{
    class AccountModel
    {
        public int ID { get; set; }
        public int Level { get; set; }

        public string Pseudo { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Strcharacters { get; set; }
        public string Strgifts { get; set; }
        public string StrFriends { get; set; }
        public string StrEnemies { get; set; }

        public long Subscription { get; set; }

        public List<string> Characters;
        public List<GiftModel> Gifts;

        public AccountModel()
        {
            Characters = new List<string>();
            Gifts = new List<GiftModel>();
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

            foreach (var datas in Strgifts.Split('+'))
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
