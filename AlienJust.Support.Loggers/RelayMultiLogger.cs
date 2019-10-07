using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers
{
	public sealed class RelayMultiLogger : ILogger {
		private readonly bool _swallowExceptions;
		private readonly ILogger[] _loggers;

		public RelayMultiLogger(bool swallowExceptions, params ILogger[] loggers) {
			_loggers = loggers;
			_swallowExceptions = swallowExceptions;
		}

		public void Log(object obj) {
			try {
				foreach (var logger in _loggers) {
					logger.Log(obj);
				}
			}
			catch {
				if (!_swallowExceptions) throw;
			}
		}
	}
}