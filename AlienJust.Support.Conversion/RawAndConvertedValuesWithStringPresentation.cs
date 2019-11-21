using AlienJust.Support.Conversion.Contracts;

namespace AlienJust.Support.Conversion
{
    public sealed class RawAndConvertedValuesWithStringPresentation<TRaw, TConverted> : IRawAndConvertedValues<TRaw, TConverted>
    {
        private readonly IBuilderOneToOne<TRaw, string> _rawStringBuilder;
        private readonly IBuilderOneToOne<TRaw, TConverted> _builder;
        private readonly IBuilderOneToOne<TConverted, string> _convertStringBuilder;
        public TRaw RawValue { get; }

        public RawAndConvertedValuesWithStringPresentation(TRaw rawValue, IBuilderOneToOne<TRaw, string> rawStringBuilder, IBuilderOneToOne<TRaw, TConverted> builder, IBuilderOneToOne<TConverted, string> convertStringBuilder)
        {
            RawValue = rawValue;
            _rawStringBuilder = rawStringBuilder;
            _builder = builder;
            _convertStringBuilder = convertStringBuilder;
        }
        public TConverted ConvertedValue => _builder.Build(RawValue);

        public override string ToString()
        {
            return (_rawStringBuilder != null ? _rawStringBuilder.Build(RawValue) : RawValue.ToString()) + " - " + (_convertStringBuilder != null ? _convertStringBuilder.Build(ConvertedValue) : ConvertedValue.ToString());
        }
    }
}