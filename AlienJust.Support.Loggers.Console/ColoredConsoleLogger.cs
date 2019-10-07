using System;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Adaptation.ConsoleLogger
{
    public class ColoredConsoleLogger : ILogger
    {
        /// <summary>
        /// sync object for all loggers
        /// </summary>
        private static readonly object Sync = new object();

        private readonly ConsoleColor _forecolor;
        private readonly ConsoleColor _backcolor;

        public ColoredConsoleLogger(ConsoleColor forecolor, ConsoleColor backcolor)
        {
            _forecolor = forecolor;
            _backcolor = backcolor;
        }

        public void Log(string text)
        {
            lock (Sync)
            {
                var currentConsoleColor = Console.ForegroundColor;
                var currentConsoleBackColor = Console.BackgroundColor;
                Console.ForegroundColor = _forecolor;
                Console.BackgroundColor = _backcolor;
                Console.WriteLine(text);
                Console.ForegroundColor = currentConsoleColor;
                Console.BackgroundColor = currentConsoleBackColor;
            }
        }

        public void Log(object obj)
        {
            Log(obj.ToString());
        }
    }
}
