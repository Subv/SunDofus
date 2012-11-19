﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SunDofus.Settings
{
    public class Configuration
    {
        Dictionary<string, string> m_elements;

        public Configuration()
        {
            m_elements = new Dictionary<string, string>();
        } 

        public void LoadConfiguration(string _path)
        {
            var reader = new StreamReader(_path);

            while (!reader.EndOfStream)
            {
                var Line = reader.ReadLine();

                if (Line.Contains("#")) 
                    continue;
                if (Line == "") 
                    continue;

                var lineInfos = Line.Split(' ');
                m_elements.Add(lineInfos[0], lineInfos[1].Replace(";", ""));
            }
        }

        public void InsertElement(string _key, string _value)
        {
            if(!m_elements.ContainsKey(_key))
                m_elements.Add(_key, _value);
        }

        public string GetStringElement(string _element)
        {
            if (!m_elements.ContainsKey(_element)) 
                return "";

            return m_elements[_element];
        }

        public int GetIntElement(string _element)
        {
            if (!m_elements.ContainsKey(_element)) 
                return -1;

            return int.Parse(m_elements[_element]);
        }

        public bool GetBoolElement(string _element)
        {
            if (!m_elements.ContainsKey(_element)) 
                return false;

            return bool.Parse(m_elements[_element]);
        }

        public long GetLongElement(string _element)
        {
            if (!m_elements.ContainsKey(_element)) 
                return -1;

            return long.Parse(m_elements[_element]);
        }
    }
}
