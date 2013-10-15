using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Auth.Entities.Models
{
    class AccountsModel
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public int Communauty { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Pseudo { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

        public DateTime SubscriptionDate { get; set; }
        public Dictionary<int, List<string>> Characters { get; set; }
        public List<string> Friends { get; set; }
        public List<string> Enemies { get; set; }

        public AccountsModel()
        {
            SubscriptionDate = new DateTime();
            Characters = new Dictionary<int, List<string>>();

            Friends = new List<string>();
            Enemies = new List<string>();
        }

        public long SubscriptionTime()
        {
            var time = SubscriptionDate.Subtract(DateTime.Now).TotalMilliseconds;

            if (Utilities.Config.GetBoolElement("SUBSCRIPTION_TIME") == false)
                return 31536000000;

            else if (SubscriptionDate.Subtract(DateTime.Now).TotalMilliseconds <= 1)
                return 0;

            return (long)time;
        }
    }
}
