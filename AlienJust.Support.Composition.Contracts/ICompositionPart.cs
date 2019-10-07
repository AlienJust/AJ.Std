namespace AlienJust.Support.Composition.Contracts
{
	public interface ICompositionPart : IUnknownLike {
		string Name { get; }
		void SetCompositionRoot(ICompositionRoot root);
		//void BecameUnused();
	}
}