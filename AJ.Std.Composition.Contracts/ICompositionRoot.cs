namespace AJ.Std.Composition.Contracts
{
	public interface ICompositionRoot {
		ICompositionPart GetPartByName(string partName);
	}
}