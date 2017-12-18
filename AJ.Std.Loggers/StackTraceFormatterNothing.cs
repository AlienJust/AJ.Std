using System.Diagnostics;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Loggers
{
	public class StackTraceFormatterNothing : IStackTraceTextFormatter {
		public string FormatStackTrace(StackTrace stackTrace, string message) {
			return message;
		}
	}
}