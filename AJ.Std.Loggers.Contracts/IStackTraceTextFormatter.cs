using System.Diagnostics;

namespace AJ.Std.Loggers.Contracts {
	public interface IStackTraceTextFormatter {
		string FormatStackTrace(StackTrace stackTrace, string message);
	}
}
