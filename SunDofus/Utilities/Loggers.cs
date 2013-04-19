using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Utilities
{
    class Loggers
    {
        public static Logger StatusLogger;
        public static Logger InfosLogger;
        public static Logger ErrorsLogger;

        public static void InitializeLoggers()
        {
            StatusLogger = new Logger("Status", Basic.Locker, ConsoleColor.Green);
            InfosLogger = new Logger("Infos", Basic.Locker, ConsoleColor.Magenta, Config.GetBoolElement("DEBUG"));
            ErrorsLogger = new Logger("Errors", Basic.Locker, ConsoleColor.Yellow);

            StatusLogger.Write(string.Format("Loggers loaded and started : MODE DEBUG = {0}", Config.GetBoolElement("DEBUG").ToString().ToUpper()));
        }

        public class Logger
        {
            private string _name;
            private object _locker;
            private bool _write;
            private ConsoleColor _color;

            public Logger(string text, object locker, ConsoleColor color, bool write = true)
            {
                _name = text;
                _locker = locker;
                _color = color;
                _write = write;
            }

            public void Write(string text, bool line = true)
            {
                if (!_write)
                    return;

                lock (_locker)
                {
                    Console.ForegroundColor = _color;
                    Console.Write(string.Format("{0} > {1} > ", DateTime.Now.ToString(), _name));
                    Console.ResetColor();
                    Console.Write(string.Format("{0}{1}", text, (line ? Environment.NewLine : "")));
                }
            }
        }
    }
}
