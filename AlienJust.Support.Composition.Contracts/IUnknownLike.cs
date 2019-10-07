namespace AlienJust.Support.Composition.Contracts
{
	public interface IUnknownLike {
		void Release();
		void AddRef();
		int RefsCount { get; }
	}
}