using System;
using System.Globalization;
using JetBrains.Annotations;

namespace DeVote.Logging
{
    /// <summary>
    ///     A "proxy" to be instantiated in classes for shorter logging calls.
    /// </summary>
    public sealed class LogProxy
    {
        public LogProxy(string classSource)
        {
            Source = classSource;
        }

        public string Source { get; private set; }

        [StringFormatMethod("format")]
        public void Info(string format, params object[] args)
        {
            LogManager.Information(Source, format, args);
        }

        [StringFormatMethod("format")]
        public void Warn(string format, params object[] args)
        {
            LogManager.Warning(Source, format, args);
        }

        [StringFormatMethod("format")]
        public void Error(string format, params object[] args)
        {
            LogManager.Error(Source, format, args);
        }

        public void Wait()
        {
            LogManager.Debug("Press a key to continue...");
            Console.ReadKey();
        }

        public void Progress(int current, int max)
        {
            lock (Console.Out)
            {
                float donePerc = (100f / max * current);
                var done = (int)System.Math.Ceiling(20f / max * current);

                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write("[" + ("".PadRight(done, '#') + "".PadLeft(20 - done, '_')) + "] {0,5}%\r",
                    donePerc.ToString("0.0", CultureInfo.InvariantCulture));
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
    }
}
