using System.Threading;
using AJ.Std.Text.Contracts;

namespace AJ.Std.Text
{
	public class ThreadFormatter : ITextFormatter {
		private readonly string _seporator;
		private readonly bool _useThreadName;
		private readonly bool _useThreadId;
		private readonly bool _usePriority;

		public ThreadFormatter(string seporator, bool useThreadName, bool useThreadId, bool usePriority) {
			_seporator = seporator;
			_useThreadName = useThreadName;
			_useThreadId = useThreadId;
			_usePriority = usePriority;
		}

		public string Format(string text) {
			if (text == string.Empty) return text;
			string result = string.Empty;
			var thread = Thread.CurrentThread;
			if (_useThreadName)
				result += thread.Name + _seporator;
			if (_useThreadId)
				result += thread.ManagedThreadId + _seporator;
			if (_usePriority)
				result += thread.Priority.AsShortString() + _seporator;

			return result + text;
		}
	}
}