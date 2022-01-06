using System.Diagnostics.Contracts;

namespace DeVote.Logging
{
    /// <summary>
    ///     The interface for any text logger.
    /// </summary>
    [ContractClass(typeof(LoggerContracts))]
    public interface ILogger
    {
        void WriteInformation(string logString);

        void WriteWarning(string logString);

        void WriteError(string logString);
    }

    [ContractClassFor(typeof(ILogger))]
    public abstract class LoggerContracts : ILogger
    {
        public void WriteInformation(string logString)
        {
            Contract.Requires(logString != null);
        }

        public void WriteWarning(string logString)
        {
            Contract.Requires(logString != null);
        }

        public void WriteError(string logString)
        {
            Contract.Requires(logString != null);
        }
    }
}
