using System;
using AlienJust.Support.Conversion.Contracts;

namespace AlienJust.Support.Conversion
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