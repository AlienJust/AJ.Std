namespace AJ.Std.Composition.Contracts
{
	public interface ICompositionPartWithInformation {
		bool IsInitComplete { get; set; }
		ICompositionPart CompositionPart { get; }
	}
}
