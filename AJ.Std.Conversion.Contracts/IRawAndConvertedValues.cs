namespace AJ.Std.Conversion.Contracts
{
	public interface IRawAndConvertedValues<out TRaw, out TConverted> {
		TRaw RawValue { get; }
		TConverted ConvertedValue { get; }
	}
}