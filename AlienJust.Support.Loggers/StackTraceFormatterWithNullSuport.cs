using System.Diagnostics;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers
{
	public class StackTraceFormatterWithNullSuport : IStackTraceTextFormatter {
		private readonly string _seporator;
		private readonly string _nullMessage;

		public StackTraceFormatterWithNullSuport(string seporator, string nullMessage) {
			_seporator = seporator;
			_nullMessage = nullMessage;
		}

		public string FormatStackTrace(StackTrace stackTrace, string message) {
			if (stackTrace == null) {
				if (string.IsNullOrEmpty(_nullMessage)) {
					return message;
				}
				return _nullMessage + _seporator + message;
			}
			var ff = stackTrace.GetFrame(0);
			return ff.GetFileName() + _seporator + ff.GetMethod().Name + _seporator + ff.GetFileLineNumber() + _seporator + message;
		}
	}
}