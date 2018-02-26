using AJ.Std.Conversion.Contracts;

namespace AJ.Std.Conversion
{
	public sealed class RawAndConvertedValuesWithStringPresentation<TRaw, TConverted> : IRawAndConvertedValues<TRaw, TConverted> {
		private readonly IBuilderOneToOne<TRaw, TConverted> _builder;
		public TRaw RawValue { get; }

		public RawAndConvertedValuesWithStringPresentation(TRaw rawValue, IBuilderOneToOne<string, TRaw> rawStringBuilder, IBuilderOneToOne<TRaw, TConverted> builder, IBuilderOneToOne<string, TRaw> convertStringBuilder) {
			_builder = builder;
			RawValue = rawValue;
		}
		public TConverted ConvertedValue => _builder.Build(RawValue);

		public override string ToString() {
			return RawValue + " - " + ConvertedValue;
		}
	}
}