using System.Globalization;
using AJ.Std.Identy.Contracts;

namespace AJ.Std.Identy {
	public sealed class IdentifierStringToLowerBased : IIdentifier {
		public IdentifierStringToLowerBased(string identyString) {
			IdentyString = identyString.ToLower(CultureInfo.InvariantCulture);
		}

		public string IdentyString { get; }

		public override string ToString() {
			return IdentyString;
		}
	}
}