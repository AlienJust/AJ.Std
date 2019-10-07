using System.Diagnostics;

namespace AlienJust.Support.Loggers.Contracts
{
	public interface ILoggerWithStackTrace {
		void Log(object obj, StackTrace stackTrace);
	}
}