using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Auth.Entities.Models
{
    class GiftsModel
    {
        private int _ID, _target, _itemID;
        private string _title, _message, _image;

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
        public int Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
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

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}", _ID, _title, _message, _itemID, _image);
        }
    }
}
