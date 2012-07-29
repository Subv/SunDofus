﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SunDofus;

namespace selector.Config
{
    class ConfigurationManager
    {
        static Configuration Config;

        public static void IniConfig()
        {
            Config = new Configuration("Config/Selector.txt");
        }

        public static string GetString(string M)
        {
            return Config.GetString(M);
        }

        public static int GetInt(string I)
        {
            return Config.GetInt(I);
        }

        public static bool GetBool(string B)
        {
            return Config.GetBool(B);
        }
    }
}
