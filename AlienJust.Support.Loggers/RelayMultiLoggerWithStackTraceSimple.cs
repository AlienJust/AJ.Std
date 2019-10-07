using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers
{
	public sealed class RelayMultiLoggerWithStackTraceSimple : IMultiLoggerWithStackTrace<int> {
		private readonly ILoggerWithStackTrace[] _loggers;

		public RelayMultiLoggerWithStackTraceSimple(params ILoggerWithStackTrace[] loggers) {
			_loggers = loggers;
		}

		public ILoggerWithStackTrace GetLogger(int loggerIndex) {
			return _loggers[loggerIndex];
		}
	}
}