using System;
using AlienJust.Support.Text.Contracts;

namespace AlienJust.Support.Text
{
	public class DateTimeFormatter : ITextFormatter {
		private readonly string _seporator;

		public DateTimeFormatter(string seporator) {
			_seporator = seporator;
		}

		public string Format(string text) {
			if (text == string.Empty) return text;
			return DateTime.Now.ToString("HH:mm:ss.fff") + _seporator + text;
		}
	}
}