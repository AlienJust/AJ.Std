using System.Diagnostics;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Loggers
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