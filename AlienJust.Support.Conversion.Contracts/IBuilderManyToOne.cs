namespace AlienJust.Support.Conversion.Contracts
{
	public interface IBuilderManyToOne<out TResult> {
		TResult Build();
	}
}