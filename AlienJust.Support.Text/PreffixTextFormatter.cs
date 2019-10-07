using AlienJust.Support.Text.Contracts;

namespace AlienJust.Support.Text
{
	public class PreffixTextFormatter : ITextFormatter {
		private readonly string _preffixText;

		public PreffixTextFormatter(string preffixText) {
			_preffixText = preffixText;
		}

		public string Format(string text) {
			return _preffixText + text;
		}
	}
}