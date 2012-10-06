﻿/*
 * ORIGINAL CLASS BY NIGHTWOLF FROM THE SNOWING's PROJECT ! All rights reserved !
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace realm.Realm.Map
{
    class Pathfinding
    {
        private string _strPath;
        private int _startCell;
        private int _startDir;

        public int Destination;
        public int NewDirection;

        private Map _map;

        public Pathfinding(string path, Map map, int startCell, int startDir)
        {
            _strPath = path;
            _map = map;
            _startCell = startCell;
            _startDir = startDir;
        }

        public string GetStartPath
        {
            get
            {
                return GetDirChar(_startDir) + GetCellChars(_startCell);
            }
        }

        public int GetCaseIDFromDirection(int caseID, char direction, bool fight)
        {
            switch (direction)
            {
                case 'a':
                    return fight ? -1 : caseID + 1;
                case 'b':
                    return caseID + _map.width;
                case 'c':
                    return fight ? -1 : caseID + (_map.width * 2 - 1);
                case 'd':
                    return caseID + (_map.width - 1);
                case 'e':
                    return fight ? -1 : caseID - 1;
                case 'f':
                    return caseID - _map.width;
                case 'g':
                    return fight ? -1 : caseID - (_map.width * 2 - 1);
                case 'h':
                    return caseID - _map.width + 1;
            }
            return -1; 
        }

        public static int GetCellNum(string CellChars)
        {

            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            int NumChar1 = hash.IndexOf(CellChars[0]) * hash.Length;
            int NumChar2 = hash.IndexOf(CellChars[1]);

            return NumChar1 + NumChar2;

        }

        public static string GetCellChars(int CellNum)
        {

            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            int CharCode2 = (CellNum % hash.Length);
            int CharCode1 = (CellNum - CharCode2) / hash.Length;

            return hash[CharCode1].ToString() + hash[CharCode2].ToString();

        }

        public static string GetDirChar(int DirNum)
        {

            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            if (DirNum >= hash.Length)
                return "";
            return hash[DirNum].ToString();

        }

        public static int GetDirNum(string DirChar)
        {

            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            return hash.IndexOf(DirChar);

        }

        public bool InLine(int cell1, int cell2)
        {
            bool isX = GetCellXCoord(cell1) == GetCellXCoord(cell2);
            bool isY = GetCellYCoord(cell1) == GetCellYCoord(cell2);
            return isX || isY;
        }

        public int NextCell(int cell, int dir)
        {
            switch (dir)
            {
                case 0:
                    return cell + 1;

                case 1:
                    return cell + _map.width;

                case 2:
                    return cell + (_map.width * 2) - 1;

                case 3:
                    return cell + _map.width - 1;

                case 4:
                    return cell - 1;

                case 5:
                    return cell - _map.width;

                case 6:
                    return cell - (_map.width * 2) + 1;

                case 7:
                    return cell - _map.width + 1;

            }
            return -1;
        }

        public string RemakeLine(int lastCell, string cell, int finalCell)
        {
            int direction = GetDirNum(cell[0].ToString());
            int toCell = GetCellNum(cell.Substring(1));
            int lenght = 0;
            if (InLine(lastCell, toCell))
            {
                lenght = GetEstimateDistanceBetween(lastCell, toCell);
            }
            else
            {
                lenght = int.Parse(Math.Truncate((GetEstimateDistanceBetween(lastCell, toCell) / 1.4)).ToString());
            }
            int backCell = lastCell;
            int actuelCell = lastCell;
            for (int i = 1; i <= lenght; i++)
            {
                actuelCell = NextCell(actuelCell, direction);
                backCell = actuelCell;
            }
            return cell + ",1";
        }

        public string RemakePath()
        {
            string newPath = "";
            int newCell = GetCellNum(_strPath.Substring(_strPath.Length - 2, 2));
            int lastCell = _startCell;
            for (int i = 0; i <= _strPath.Length - 1; i += 3)
            {
                string actualCell = _strPath.Substring(i, 3);
                string[] lineData = RemakeLine(lastCell, actualCell, newCell).Split(',');
                newPath += lineData[0];
                if (lineData[1] == null)
                    return newPath;
                lastCell = GetCellNum(actualCell.Substring(1));
            }
            Destination = GetCellNum(_strPath.Substring(_strPath.Length - 2, 2));
            NewDirection = GetDirNum(_strPath.Substring(_strPath.Length - 3, 1));
            return newPath;
        }

        public int GetDistanceBetween(int id1, int id2)
        {
            if (id1 == id2) return 0;
            if (_map == null) return 0;
            int diffX = Math.Abs(GetCellXCoord(id1) - GetCellXCoord(id2));
            int diffY = Math.Abs(GetCellYCoord(id1) - GetCellYCoord(id2));
            return (diffX + diffY);
        }

        public int GetEstimateDistanceBetween(int id1, int id2)
        {
            if (id1 == id2) return 0;
            if (_map == null) return 0;
            int diffX = Math.Abs(GetCellXCoord(id1) - GetCellXCoord(id2));
            int diffY = Math.Abs(GetCellYCoord(id1) - GetCellYCoord(id2));
            return int.Parse(Math.Truncate(Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2))).ToString());
        }

        public int GetCellXCoord(int cellid)
        {
            int w = _map.width;
            return ((cellid - (w - 1) * GetCellYCoord(cellid)) / w);
        }

        public int GetCellYCoord(int cellid)
        {
            int w = _map.width;
            int loc5 = (int)(cellid / ((w * 2) - 1));
            int loc6 = cellid - loc5 * ((w * 2) - 1);
            int loc7 = loc6 % w;
            return (loc5 - loc7);
        }
    }
}
