namespace AlienJust.Support.Composition.Contracts
{
	public interface ICompositionRoot
	{
		ICompositionPart GetPartByName(string partName);
	}
}