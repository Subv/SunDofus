using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Entities.Models.Items;

namespace SunDofus.World.Game.Characters.Items
{
    class ItemsHandler
    {
        private static int _lastID = 0;

        public static int GetNewID()
        {
            return ++_lastID;
        }

        public static bool PositionAvaliable(int itemType, bool usable, int position)
        {

            return true;

        }

        public static bool ConditionsAvaliable(ItemModel item, Character character)
        {
            var condi = item.Condistr;
            var avaliable = false;

            if (condi == "")
                return true;

            foreach(var cond in condi.Split('&'))
            {
                var spliter = cond.Substring(2, 1);
                long value = -1;
                var toCompare = int.Parse(cond.Substring(3));

                switch(cond.Substring(0,1))
                {
                    case "C":

                        switch(cond.Substring(1,1))
                        {
                            case "a":
                                value = character.Stats.agility.Bases;
                                break;

                            case "i":
                                value = character.Stats.intelligence.Bases;
                                break;

                            case "c":
                                value = character.Stats.luck.Bases;
                                break;

                            case "s":
                                value = character.Stats.strenght.Bases;
                                break;

                            case "v":
                                value = character.Stats.life.Bases;
                                break;

                            case "w":
                                value = character.Stats.wisdom.Bases;
                                break;

                            case "A":
                                value = character.Stats.agility.Total();
                                break;

                            case "I":
                                value = character.Stats.intelligence.Total();
                                break;

                            case "C":
                                value = character.Stats.luck.Total();
                                break;

                            case "S":
                                value = character.Stats.strenght.Total();
                                break;

                            case "V":
                                value = character.Stats.life.Total();
                                break;

                            case "W":
                                value = character.Stats.wisdom.Total();
                                break;

                            default:
                                avaliable = true;
                                break;
                        }

                        break;

                    case "P":

                        switch(cond.Substring(1,1))
                        {
                            case "G":
                                value = character.Class;
                                break;

                            case "L":
                                value = character.Level;
                                break;

                            case "K":
                                value = character.Kamas;
                                break;

                            default:
                                avaliable = true;
                                break;
                        }

                        break;

                    default:
                        avaliable = true;
                        break;
                }

                if (avaliable == true) 
                    return true;

                if (spliter != "")
                {
                    switch (spliter)
                    {
                        case "<":
                            avaliable = (value < toCompare ? true : false);
                            break;

                        case ">":
                            avaliable = (value > toCompare ? true : false);
                            break;

                        case "=":
                            avaliable = (value == toCompare ? true : false);
                            break;

                        case "~":
                            avaliable = (value == toCompare ? true : false);
                            break;

                        case "!":
                            avaliable = (value != toCompare ? true : false);
                            break;

                        default:
                            avaliable = true;
                            break;
                    }

                    if (avaliable == false)
                        return false;
                }
            }

            return avaliable;
        }
    }
}
