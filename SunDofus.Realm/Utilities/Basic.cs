﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace realm.Utilities
{
    class Basic
    {
        public static Random _Rando = new Random();
        public static object _Locker = new object();

        public static string Vowels = "aeiouy";
        public static string Consonants = "bcdfghjklmnpqrstvwxz";

        public static int Rand(int Min, int Max)
        {
            return _Rando.Next(Min, Max + 1);
        }

        public static string GetVowels()
        {
            return Vowels.Substring((Rand(0, Vowels.Length - 1)), 1);
        }

        public static string GetConsonants()
        {
            return Consonants.Substring((Rand(0, Consonants.Length - 1)), 1);
        }

        public static string RandomName()
        {
            var Name = "";

            Name += GetConsonants();
            Name += GetVowels();
            Name += GetVowels();
            Name += GetConsonants();
            Name += GetConsonants();
            Name += GetVowels();

            if (Rand(0, 1) == 0)
            {
                Name += GetConsonants();
                Name += GetVowels();
            }

            return Name;
        }

        public static string DeciToHex(int Deci)
        {
            if (Deci == -1) return "-1";
            else return Deci.ToString("x");
        }

        public static int HexToDeci(string Hex)
        {
            if (Hex == "-1" | Hex == "") return -1;
            return Convert.ToInt32(Hex, 16);
        }

        public static string GetActuelTime()
        {
            return string.Format("{0}{1}{2}{3}", (DateTime.Now.Hour * 3600000), (DateTime.Now.Minute * 60000),
                (DateTime.Now.Second * 1000), DateTime.Now.Millisecond.ToString());
        }

        public static string GetDofusDate()
        {
            return string.Format("BD{0}|{1}|{2}", (DateTime.Now.Year - 1370).ToString(), (DateTime.Now.Month - 1), (DateTime.Now.Day));
        }

        public static int GetRandomJet(string JetStr, int Jet = 3)
        {
            if (JetStr.Length > 3)
            {
                var Damage = 0;
                var DS = int.Parse(JetStr.Split('d')[0]);
                var Faces = int.Parse(JetStr.Split('d')[1].Split('+')[0]);
                var Fixe = int.Parse(JetStr.Split('d')[1].Split('+')[1]);

                if (DS != 0)
                {
                    for (var i = 1; i <= DS; i++)
                    {
                        if (Jet == 1)
                            Damage += Faces;
                        else if (Jet == 2)
                            Damage += 1;
                        else if (Jet == 3)
                            Damage += (int)Math.Ceiling((double)(Faces / 2));
                        else
                            Damage += Rand(1, Faces);
                    }
                }
                    return (Damage + Fixe);
            }
            else
                return 0;
        }
    }
}
