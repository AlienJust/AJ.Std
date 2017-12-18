using System.Diagnostics;

namespace AJ.Std.Loggers.Contracts
{
	public interface ILoggerWithStackTrace {
		void Log(object obj, StackTrace stackTrace);
	}
}