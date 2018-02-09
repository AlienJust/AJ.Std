namespace AJ.Std.Conversion.Contracts
{
	public interface IBuilderManyToOne<out TResult> {
		TResult Build();
	}
}