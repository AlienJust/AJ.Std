namespace AJ.Std.Conversion.Contracts
{
	public interface IBuilderOneToOne<in TSource, out TResult> {
		TResult Build(TSource source);
	}
}
