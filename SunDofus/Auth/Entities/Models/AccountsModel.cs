using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Auth.Entities.Models
{
    class AccountsModel
    {
        private int _ID;
        private int _level;
        private int _communauty;

        private string _username;
        private string _password;
        private string _pseudo;
        private string _question;
        private string _answer;

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
        public int Communauty
        {
            get
            {
                return _communauty;
            }
            set
            {
                _communauty = value;
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
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

        public DateTime SubscriptionDate;
        public Dictionary<int, List<string>> Characters;

        public AccountsModel()
        {
            SubscriptionDate = new DateTime();
            Characters = new Dictionary<int, List<string>>();
        }

        public long SubscriptionTime()
        {
            var time = SubscriptionDate.Subtract(DateTime.Now).TotalMilliseconds;

            if (Utilities.Config.GetBoolElement("Subscription_Time") == false)
                return 31536000000;

            else if (SubscriptionDate.Subtract(DateTime.Now).TotalMilliseconds <= 1)
                return 0;

            return (long)time;
        }
    }
}
