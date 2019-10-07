namespace AlienJust.Support.Identy.Contracts {
	/// <summary>
	/// Identifier, allows to identify something (now only  by string comparison)
	/// </summary>
	public interface IIdentifier {
		string IdentyString { get; }
	}
}
