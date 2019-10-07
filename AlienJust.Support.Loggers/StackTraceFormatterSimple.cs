using System.Diagnostics;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers
{
	public class StackTraceFormatterSimple : IStackTraceTextFormatter {
		private readonly string _seporator;
		public StackTraceFormatterSimple(string seporator) {
			_seporator = seporator;
		}

		public string FormatStackTrace(StackTrace stackTrace, string message) {
			var ff = stackTrace.GetFrame(0);
			return ff.GetFileName() + _seporator + ff.GetMethod().Name + _seporator + ff.GetFileLineNumber() + _seporator + message;
		}
	}
}