using AJ.Std.Text.Contracts;

namespace AJ.Std.Text
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