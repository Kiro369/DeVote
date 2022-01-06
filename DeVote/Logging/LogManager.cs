using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using DeVote.Logging.Loggers;

namespace DeVote.Logging
{
    /// <summary>
    ///     Takes care of all logging in the entire source base.
    ///     Do not use this class directly (except for debug logging); use LogProxy objects instead.
    /// </summary>
    public static class LogManager
    {
        private static readonly List<ILogger> Loggers = new List<ILogger>
        {
            // TODO: Make this more customizable.
            new ConsoleLogger()
        };

        private static readonly StringBuilder Builder = new StringBuilder();
        private static readonly object Lock = new object();

        public static void AddLogger(ILogger logger)
        {
            Contract.Requires(logger != null);

            Loggers.Add(logger);
        }

        private static string PrepareForOutput(string source, string logString, params object[] args)
        {
            Builder.Append("[");
            Builder.Append(source);
            Builder.Append("] ");
            Builder.AppendFormat(CultureInfo.InvariantCulture, logString, args);

            string result = Builder.ToString();
            Contract.Assume(!string.IsNullOrEmpty(result));
            Builder.Clear();

            return result;
        }

        internal static void Information(string source, string logString, params object[] args)
        {
            lock (Lock)
                foreach (ILogger log in Loggers)
                    log.WriteInformation(PrepareForOutput(source, logString, args));
        }

        internal static void Warning(string source, string logString, params object[] args)
        {
            lock (Lock)
                foreach (ILogger log in Loggers)
                    log.WriteWarning(PrepareForOutput(source, logString, args));
        }

        internal static void Error(string source, string logString, params object[] args)
        {
            lock (Lock)
                foreach (ILogger log in Loggers)
                    log.WriteError(PrepareForOutput(source, logString, args));
        }

        [Conditional("DEBUG")]
        public static void Debug(string logString, params object[] args)
        {
            Contract.Requires(logString != null);
            Contract.Requires(args != null);

            lock (Lock)
                Console.WriteLine(logString, args);
        }
    }
}
