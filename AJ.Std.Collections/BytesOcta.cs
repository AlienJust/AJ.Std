using System;

namespace AJ.Std.Collections {
	/// <summary>
	/// Bytes octave
	/// </summary>
	public struct BytesOcta {
		/// <summary>
		/// First byte
		/// </summary>
		public byte First { get; }

		/// <summary>
		/// Second byte
		/// </summary>
		public byte Second { get; }

		/// <summary>
		/// Third byte
		/// </summary>
		public byte Third { get; }

		/// <summary>
		/// Fourth byte
		/// </summary>
		public byte Fourth { get; }

		/// <summary>
		/// Fifth byte
		/// </summary>
		public byte Fifth { get; }

		/// <summary>
		/// Sixth byte
		/// </summary>
		public byte Sixth { get; }

		/// <summary>
		/// Seventh byte
		/// </summary>
		public byte Seventh { get; }

		/// <summary>
		/// Eighth byte
		/// </summary>
		public byte Eighth { get; }


		public BytesOcta(byte first, byte second, byte third, byte fourth, byte fifth, byte sixth, byte seventh, byte eighth) {
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
			Fifth = fifth;
			Sixth = sixth;
			Seventh = seventh;
			Eighth = eighth;
		}

		public static BytesOcta FromSignedLongHighFirst(long value) {
			byte hf8 = (byte)((value & unchecked((long)0xFF000000_00000000)) >> 56);
			byte hf7 = (byte)((value & 0x00FF0000_00000000) >> 48);
			byte hf6 = (byte)((value & 0x0000FF00_00000000) >> 40);
			byte hf5 = (byte)((value & 0x000000FF_00000000) >> 32);
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesOcta(hf8, hf7, hf6, hf5, uhi, ulo, hi, lo);
		}

		public static BytesOcta FromSignedLongLowFirst(long value) {
			byte hf8 = (byte)((value & unchecked((long)0xFF000000_00000000)) >> 56);
			byte hf7 = (byte)((value & 0x00FF0000_00000000) >> 48);
			byte hf6 = (byte)((value & 0x0000FF00_00000000) >> 40);
			byte hf5 = (byte)((value & 0x000000FF_00000000) >> 32);
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesOcta(lo, hi, ulo, uhi, hf5, hf6, hf7, hf8);
		}

		public static BytesOcta FromUnsignedLongHighFirst(ulong value) {
			byte hf8 = (byte)((value & 0xFF000000_00000000) >> 56);
			byte hf7 = (byte)((value & 0x00FF0000_00000000) >> 48);
			byte hf6 = (byte)((value & 0x0000FF00_00000000) >> 40);
			byte hf5 = (byte)((value & 0x000000FF_00000000) >> 32);
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesOcta(hf8, hf7, hf6, hf5, uhi, ulo, hi, lo);
		}

		public static BytesOcta FromUnsignedLongLowFirst(ulong value) {
			byte hf8 = (byte)((value & 0xFF000000_00000000) >> 56);
			byte hf7 = (byte)((value & 0x00FF0000_00000000) >> 48);
			byte hf6 = (byte)((value & 0x0000FF00_00000000) >> 40);
			byte hf5 = (byte)((value & 0x000000FF_00000000) >> 32);
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesOcta(lo, hi, ulo, uhi, hf5, hf6, hf7, hf8);
		}

		/// <summary>
		/// Return platform depended (according to local machine CPU architecture little endian or big endian) array for further conversion with BitConverter class
		/// </summary>
		/// <returns>Array for further processing with BitConverter</returns>
		public static byte[] GetArrayForBitConverterAccodringToCurrentArchitectureEndian(byte first, byte second, byte third, byte fourth, byte fifth, byte sixth, byte seventh, byte eighth) {
			if (BitConverter.IsLittleEndian) {
				return new[] { eighth, seventh, sixth, fifth, fourth, third, second, first };
			}

			return new[] { first, second, third, fourth, fifth, sixth, seventh, eighth };
		}

		/// <summary>
		/// Returns this structure as ulong using first byte as high
		/// </summary>
		public ulong HighFirstUnsignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(First, Second, Third, Fourth, Fifth, Sixth, Seventh, Eighth);
				return BitConverter.ToUInt64(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns this structure as long using first byte as high
		/// </summary>
		public long HighFirstSignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(First, Second, Third, Fourth, Fifth, Sixth, Seventh, Eighth);
				return BitConverter.ToInt64(tempByteArray, 0);
			}
		}


		/// <summary>
		/// Returns this structure as ulong using first byte as low
		/// </summary>
		public ulong LowFirstUnsignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(Eighth, Seventh, Sixth, Fifth, Fourth, Third, Second, First);
				return BitConverter.ToUInt32(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns this structure as long using first byte as low
		/// </summary>
		public long LowFirstSignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(Eighth, Seventh, Sixth, Fifth, Fourth, Third, Second, First);
				return BitConverter.ToInt32(tempByteArray, 0);
			}
		}

		public override bool Equals(object obj) {
			return obj is BytesOcta octa && this == octa;
		}

		public override int GetHashCode() {
			return First.GetHashCode() ^ Second.GetHashCode();
		}

		public static bool operator ==(BytesOcta x, BytesOcta y) {
			return x.First == y.First && x.Second == y.Second && x.Third == y.Third && x.Fourth == y.Fourth && x.Fifth == y.Fifth && x.Sixth == y.Sixth && x.Seventh == y.Seventh && x.Eighth == y.Eighth;
		}

		public static bool operator !=(BytesOcta x, BytesOcta y) {
			return !(x == y);
		}

		/// <summary>
		/// Returns BCD value of the structure using first byte as high
		/// </summary>
		public long HighFirstBcd =>
			First.AsBcd() * 100_000_000_000_000 +
			Second.AsBcd() * 1_000_000_000_000 +
			Third.AsBcd() * 10_000_000_000 +
			Fourth.AsBcd() * (long)100_000_000 +
			Fifth.AsBcd() * (long)1_000_000 +
			Sixth.AsBcd() * (long)10_000 +
			Seventh.AsBcd() * (long)100 +
			Eighth.AsBcd();

		/// <summary>
		/// Returns BCD value of the structure using first byte as low
		/// </summary>
		public long LowFirstBcd =>
			Eighth.AsBcd() * 100000000000000 +
			Seventh.AsBcd() * 1000000000000 +
			Sixth.AsBcd() * 10000000000 +
			Fifth.AsBcd() * (long)100000000 +
			Fourth.AsBcd() * (long)1000000 +
			Third.AsBcd() * (long)10000 +
			Second.AsBcd() * (long)100 +
			First.AsBcd();

		/// <summary>
		/// Creates structure from BCD value using first byte as high
		/// </summary>
		/// <param name="bcdValueHf">BCD value</param>
		/// <returns>New octave</returns>
		public static BytesOcta FromBcdHighFirst(long bcdValueHf) {
			//if (bcdValueHf < 0 || bcdValueHf > 9999_9999__9999_9999) throw new ArgumentOutOfRangeException(nameof(bcdValueHf), "must be in range [0-9999_9999__9999_9999]");
			long binHighFirst = 0;
			for (int digit = 0; digit < 16; ++digit) {
				long nibble = bcdValueHf % 10;
				binHighFirst |= nibble << (digit * 4);
				bcdValueHf /= 10;
			}
			return FromSignedLongHighFirst(binHighFirst);
		}

		/// <summary>
		/// Creates structure from BCD value using first byte as low
		/// </summary>
		/// <param name="bcdValueLf">BCD value</param>
		/// <returns>New octave</returns>
		public static BytesOcta FromBcdLowFirst(long bcdValueLf) {
			//if (bcdValueLf < 0 || bcdValueLf > 9999_9999__9999_9999) throw new ArgumentOutOfRangeException(nameof(bcdValueLf), "must be in range [0-9999_9999__9999_9999]");
			long binLowFirst = 0;
			for (int digit = 0; digit < 16; ++digit) {
				long nibble = bcdValueLf % 10;
				binLowFirst |= nibble << (digit * 4);
				bcdValueLf /= 10;
			}
			return FromSignedLongLowFirst(binLowFirst);
		}

		public override string ToString() {
			return First.ToString("X2") + " " + Second.ToString("X2") + " " + Third.ToString("X2") + " " + Fourth.ToString("X2") + " " + Fifth.ToString("X2") + " " + Sixth.ToString("X2") + " " + Seventh.ToString("X2") + " " + Eighth.ToString("X2");
		}
	}
}