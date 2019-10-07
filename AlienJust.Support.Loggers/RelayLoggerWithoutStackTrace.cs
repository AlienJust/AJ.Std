using System.Diagnostics;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers
{
	public class RelayLoggerWithoutStackTrace : ILoggerWithStackTrace {
		private readonly ILogger _relayLogger;

		public RelayLoggerWithoutStackTrace(ILogger relayLogger) {
			_relayLogger = relayLogger;
		}

		public void Log(object obj, StackTrace stackTrace) {
			_relayLogger.Log(obj);
		}
	}
}