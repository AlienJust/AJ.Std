using System.Diagnostics;

namespace AlienJust.Support.Loggers.Contracts {
	public interface IStackTraceTextFormatter {
		string FormatStackTrace(StackTrace stackTrace, string message);
	}
}
