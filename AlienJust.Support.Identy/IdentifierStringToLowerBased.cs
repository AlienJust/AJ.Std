using System.Globalization;
using AlienJust.Support.Identy.Contracts;

namespace AlienJust.Support.Identy {
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