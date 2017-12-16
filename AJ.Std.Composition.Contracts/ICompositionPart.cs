namespace AJ.Std.Composition.Contracts
{
	public interface ICompositionPart : IUnknownLike {
		string Name { get; }
		void SetCompositionRoot(ICompositionRoot root);
		//void BecameUnused();
	}
}