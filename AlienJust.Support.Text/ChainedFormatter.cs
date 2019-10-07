using System.Collections.Generic;
using System.Linq;
using AlienJust.Support.Text.Contracts;

namespace AlienJust.Support.Text
{
	public class ChainedFormatter : ITextFormatter {
		private readonly IEnumerable<ITextFormatter> _formatters;
		public ChainedFormatter(IEnumerable<ITextFormatter> formatters) {
			_formatters = formatters;
		}
		public string Format(string text) {
			return _formatters.Aggregate(text, (current, f) => f.Format(current));
		}
	}
}
