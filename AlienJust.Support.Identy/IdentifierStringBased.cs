using AlienJust.Support.Identy.Contracts;

namespace AlienJust.Support.Identy {
	public sealed class IdentifierStringBased : IIdentifier {
		public IdentifierStringBased(string identyString) {
			IdentyString = identyString;
		}

		public string IdentyString { get; }

		public override string ToString() {
			return IdentyString;
		}
	}
}