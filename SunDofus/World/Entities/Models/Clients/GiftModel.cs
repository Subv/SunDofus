using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Realm.Characters.Items;

namespace SunDofus.World.Entities.Models.Clients
{
    class GiftModel
    {
        private int _ID;
        private int _itemID;

        private string _title;
        private string _message;
        private string _image;

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
        public int ItemID
        {
            get
            {
                return _itemID;
            }
            set
            {
                _itemID = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public CharacterItem Item;
    }
}
