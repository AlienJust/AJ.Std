using System;
using AlienJust.Support.Loggers.Contracts;
using AlienJust.Support.Text.Contracts;

namespace AlienJust.Support.Loggers
{
	public sealed class RelayLogger : ILogger {
		private readonly ILogger _relayLogger;
		private readonly ITextFormatter _textFormatter;
		private readonly Action<string> _selectedLogAction;

		public RelayLogger(ILogger relayLogger) {
			_relayLogger = relayLogger;
			_textFormatter = null;
			_selectedLogAction = _relayLogger == null ? (Action<string>)LogNothing : LogSimple;
		}

		public RelayLogger(ILogger relayLogger, ITextFormatter textFormatter) {
			_relayLogger = relayLogger;
			_textFormatter = textFormatter;
			_selectedLogAction = _relayLogger == null ? LogNothing : _textFormatter == null ? (Action<string>)LogSimple : LogAdvanced;
		}

		public void Log(object obj) {
			_selectedLogAction(obj.ToString());
		}


		private void LogNothing(string text) {
		}

		private void LogSimple(string text) {
			_relayLogger.Log(text);
		}

		private void LogAdvanced(string text) {
			_relayLogger.Log(_textFormatter.Format(text));
		}
	}
}