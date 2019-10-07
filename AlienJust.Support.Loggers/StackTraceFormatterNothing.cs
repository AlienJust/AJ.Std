using System.Diagnostics;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers
{
	public class StackTraceFormatterNothing : IStackTraceTextFormatter {
		public string FormatStackTrace(StackTrace stackTrace, string message) {
			return message;
		}
	}
}