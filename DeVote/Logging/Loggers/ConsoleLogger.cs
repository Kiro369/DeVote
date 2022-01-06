using System;
using System.Collections.Generic;
using System.Text;

namespace DeVote.Logging.Loggers
{
    /// <summary>
    ///     A logger that outputs text to the console.
    /// </summary>
    internal sealed class ConsoleLogger : ILogger
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void WriteInformation(string logString)
        {
            WriteToConsole(logString, GetColor(ConsoleColor.Green));
        }

        public void WriteWarning(string logString)
        {
            WriteToConsole(logString, GetColor(ConsoleColor.Yellow));
        }

        public void WriteError(string logString)
        {
            WriteToConsole(logString, GetColor(ConsoleColor.Red));
        }

        private static ConsoleColor? GetColor(ConsoleColor color)
        {
            return (ConsoleColor?)color;
        }

        private void WriteToConsole(string str, ConsoleColor? color)
        {
            lock (Console.Out)
            {
                if (color != null)
                    Console.ForegroundColor = color.Value;

                _builder.Append("[").Append(DateTime.Now).Append("] ");

                _builder.Append(str);
                Console.WriteLine(_builder.ToString());
                _builder.Clear();

                if (color != null)
                    Console.ResetColor();
            }
        }
    }
}
