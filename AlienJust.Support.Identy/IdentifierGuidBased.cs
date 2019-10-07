using System;
using AlienJust.Support.Identy.Contracts;

namespace AlienJust.Support.Identy {
	// System depended
	public class IdentifierGuidBased : IIdentifier {
		private readonly Guid _guid;

		public IdentifierGuidBased(Guid guid) {
			_guid = guid;
		}

		public override string ToString() {
			return _guid.ToString().ToLower();
		}

		public string IdentyString => ToString();
	}
}
