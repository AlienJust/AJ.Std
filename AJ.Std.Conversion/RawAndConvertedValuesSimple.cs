using System;
using AJ.Std.Conversion.Contracts;

namespace AJ.Std.Conversion
{
	public sealed class RawAndConvertedValuesSimple<TRaw, TConverted> : IRawAndConvertedValues<TRaw, TConverted> {
		public TRaw RawValue { get; }
		public TConverted ConvertedValue { get; }

		public RawAndConvertedValuesSimple(TRaw rawValue, TConverted convertedValue) {
			RawValue = rawValue;
			ConvertedValue = convertedValue;
		}

		public override string ToString() {
			try {
				return RawValue + " - " + ConvertedValue;
			}
			catch (Exception e) {
				return e.ToString();
			}
		}
	}
}