using System;
using System.Collections.Generic;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Loggers
{
	public sealed class RelayMultiLoggerWithStackTraceCustom<T> : IMultiLoggerWithStackTrace<T> {
		private readonly Dictionary<T, ILoggerWithStackTrace> _loggers;
		public RelayMultiLoggerWithStackTraceCustom(params Tuple<T, ILoggerWithStackTrace>[] loggers) {
			_loggers = new Dictionary<T, ILoggerWithStackTrace>();
			foreach (var logger in loggers) {
				_loggers.Add(logger.Item1, logger.Item2);
			}
		}

		public ILoggerWithStackTrace GetLogger(T loggerIndex) {
			return _loggers[loggerIndex];
		}
	}
}