using System;
using System.Globalization;

namespace AJ.Std.Collections
{
	/// <summary>
	/// Pair of bytes
	/// </summary>
	public struct BytesPair {
		/// <summary>
		/// First byte
		/// </summary>
		public byte First { get; }

		/// <summary>
		/// Second byte
		/// </summary>
		public byte Second { get; }

		public BytesPair(byte first, byte second) {
			First = first;
			Second = second;
		}

		/// <summary>
		/// Returns array of bytes according to CPU architecture byte order (big endian, or little endian) for further conversion with BitConverter
		/// </summary>
		/// <param name="first">First byte</param>
		/// <param name="second">Second byte</param>
		/// <returns>Array of bytes for further conversion with BitConverter</returns>
		public static byte[] GetArrayForBitConverterAccodringToCurrentArchitectureEndian(byte first, byte second) {
			if (BitConverter.IsLittleEndian) {
				return new[] { second, first };
			}
			return new[] { first, second };
		}

		/// <summary>
		/// Returns value of this structure as unsigned 16bit integer where first byte is high
		/// </summary>
		public ushort HighFirstUnsignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(First, Second);
				return BitConverter.ToUInt16(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns value of this structure as signed 16bit integer where first byte is high
		/// </summary>
		public short HighFirstSignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(First, Second);
				return BitConverter.ToInt16(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns value of this structure as unsigned 16bit integer where first byte is low
		/// </summary>
		public ushort LowFirstUnsignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(Second, First);
				return BitConverter.ToUInt16(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns value of this structure as signed 16bit integer where first byte is low
		/// </summary>
		public short LowFirstSignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(Second, First);
				return BitConverter.ToInt16(tempByteArray, 0);
			}
		}

		public override bool Equals(object obj) {
			return obj is BytesPair pair && this == pair;
		}
		public override int GetHashCode() {
			return First.GetHashCode() ^ Second.GetHashCode();
		}
		public static bool operator ==(BytesPair x, BytesPair y) {
			return x.First == y.First && x.Second == y.Second;
		}
		public static bool operator !=(BytesPair x, BytesPair y) {
			return !(x == y);
		}

		public static BytesPair FromSignedShortHighFirst(short value) {
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesPair(hi, lo);
		}

		public static BytesPair FromSignedShortLowFirst(short value) {
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesPair(lo, hi);
		}

		public static BytesPair FromUnsignedShortHighFirst(ushort value) {
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesPair(hi, lo);
		}

		public static BytesPair FromUnsignedShortLowFirst(ushort value) {
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesPair(lo, hi);
		}

		/// <summary>
		/// Creates structure from BCD and using first byte as high
		/// </summary>
		/// <param name="bcdValueHf">BCD value</param>
		/// <returns>New structure instance</returns>
		public static BytesPair ToBcdHighFirst(int bcdValueHf) {
			if (bcdValueHf < 0 || bcdValueHf > 9999) throw new ArgumentOutOfRangeException(nameof(bcdValueHf), "Must be in range 0-9999");
			int bcd = 0;
			for (int digit = 0; digit < 4; ++digit) {
				int nibble = bcdValueHf % 10;
				bcd |= nibble << (digit * 4);
				bcdValueHf /= 10;
			}
			return new BytesPair((byte)((bcd >> 8) & 0xff), (byte)(bcd & 0xff));
		}

		/// <summary>
		/// Creates structure from BCD and using first byte as low
		/// </summary>
		/// <param name="bcdValueLf">BCD value</param>
		/// <returns>New structure instance</returns>
		public static BytesPair ToBcdLowFirst(int bcdValueLf) {
			if (bcdValueLf < 0 || bcdValueLf > 9999) throw new ArgumentOutOfRangeException(nameof(bcdValueLf), "Must be in range 0-9999");
			int bcd = 0;
			for (int digit = 0; digit < 4; ++digit) {
				int nibble = bcdValueLf % 10;
				bcd |= nibble << (digit * 4);
				bcdValueLf /= 10;
			}
			return new BytesPair((byte)(bcd & 0xff), (byte)((bcd >> 8) & 0xff));
		}

		public override string ToString() {
			return First.ToString("X2") + Second.ToString("X2");
		}

		public static BytesPair Parse(string value) {
			if (value == null) throw new NullReferenceException("Input string must be not null");
			if (value.Length != 4) throw new Exception("Supported length of the string is 4");
			var first = byte.Parse(value.Substring(0, 2), NumberStyles.HexNumber);
			var second = byte.Parse(value.Substring(2, 2), NumberStyles.HexNumber);
			return new BytesPair(first, second);
		}

		public int HighFirstBcd => First.ToBcdInteger() * 100 + Second.ToBcdInteger();
		public int LowFirstBcd => Second.ToBcdInteger() * 100 + First.ToBcdInteger();
	}
}