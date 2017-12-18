using System.Collections.Generic;
using System.Linq;
using AJ.Std.Text.Contracts;

namespace AJ.Std.Text
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
