﻿using System.Diagnostics;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Loggers
{
	public class RelayLoggerWithStackTrace : ILoggerWithStackTrace {
		private readonly ILogger _relayLogger;
		private readonly IStackTraceTextFormatter _stackTraceTextFormatter;

		public RelayLoggerWithStackTrace(ILogger relayLogger, IStackTraceTextFormatter stackTraceTextFormatter) {
			_relayLogger = relayLogger;
			_stackTraceTextFormatter = stackTraceTextFormatter;
		}

		public void Log(object obj, StackTrace stackTrace) {
			_relayLogger.Log(_stackTraceTextFormatter.FormatStackTrace(stackTrace, obj.ToString()));
		}
	}
}