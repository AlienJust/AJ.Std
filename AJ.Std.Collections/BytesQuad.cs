using System;

namespace AJ.Std.Collections
{
	/// <summary>
	/// Quad of bytes
	/// </summary>
	public struct BytesQuad {
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


		public BytesQuad(byte first, byte second, byte third, byte fourth) {
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
		}

		public static BytesQuad FromSignedIntHighFirst(int value) {
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesQuad(uhi, ulo, hi, lo);
		}

		public static BytesQuad FromSignedIntLowFirst(int value) {
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesQuad(lo, hi, ulo, uhi);
		}

		public static BytesQuad FromUnsignedIntHighFirst(uint value) {
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesQuad(uhi, ulo, hi, lo);
		}

		public static BytesQuad FromUnsignedIntLowFirst(uint value) {
			byte uhi = (byte)((value & 0xFF000000) >> 24);
			byte ulo = (byte)((value & 0xFF0000) >> 16);
			byte hi = (byte)((value & 0xFF00) >> 8);
			byte lo = (byte)(value & 0xFF);
			return new BytesQuad(lo, hi, ulo, uhi);
		}

		/// <summary>
		/// Returns array of bytes according to CPU architecture's byte order (big endian, or little endian) for further conversion with BitConverter
		/// </summary>
		/// <param name="first">First byte</param>
		/// <param name="second">Second byte</param>
		/// <param name="third">Third byte</param>
		/// <param name="fourth">Fourth byte</param>
		/// <returns>Array of bytes for further conversion with BitConverter</returns>
		public static byte[] GetArrayForBitConverterAccodringToCurrentArchitectureEndian(byte first, byte second, byte third, byte fourth) {
			if (BitConverter.IsLittleEndian) {
				return new[] { fourth, third, second, first };
			}
			return new[] { first, second, third, fourth };
		}

		/// <summary>
		/// Returns value of this structure as unsigned 32bit integer where first byte is high
		/// </summary>
		public uint HighFirstUnsignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(First, Second, Third, Fourth);
				return BitConverter.ToUInt32(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns value of this structure as signed 32bit integer where first byte is high
		/// </summary>
		public int HighFirstSignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(First, Second, Third, Fourth);
				return BitConverter.ToInt32(tempByteArray, 0);
			}
		}


		/// <summary>
		/// Returns value of this structure as unsigned 32bit integer where first byte is low
		/// </summary>
		public uint LowFirstUnsignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(Fourth, Third, Second, First);
				return BitConverter.ToUInt32(tempByteArray, 0);
			}
		}

		/// <summary>
		/// Returns value of this structure as signed 32bit integer where first byte is low
		/// </summary>
		public int LowFirstSignedValue {
			get {
				var tempByteArray = GetArrayForBitConverterAccodringToCurrentArchitectureEndian(Fourth, Third, Second, First);
				return BitConverter.ToInt32(tempByteArray, 0);
			}
		}

		public override bool Equals(object obj) {
			return obj is BytesQuad && this == (BytesQuad)obj;
		}
		public override int GetHashCode() {
			return First.GetHashCode() ^ Second.GetHashCode();
		}
		public static bool operator ==(BytesQuad x, BytesQuad y) {
			return x.First == y.First && x.Second == y.Second && x.Third == y.Third && x.Fourth == y.Fourth;
		}
		public static bool operator !=(BytesQuad x, BytesQuad y) {
			return !(x == y);
		}

		/// <summary>
		/// Возвращает BCD значение структуры  считая первый байт старшим
		/// Returns value of structure as BCD value using first byte as high
		/// </summary>
		public int HighFirstBcd => First.ToBcdInteger() * 1000000 + Second.ToBcdInteger() * 10000 + Third.ToBcdInteger() * 100 + Fourth.ToBcdInteger();

		/// <summary>
		/// Returns value of structure as BCD value using first byte as low
		/// </summary>
		public int LowFirstBcd => Fourth.ToBcdInteger() * 1000000 + Third.ToBcdInteger() * 10000 + Second.ToBcdInteger() * 100 + First.ToBcdInteger();

		/// <summary>
		/// Creates structure from BCD and using first byte as high
		/// </summary>
		/// <param name="bcdValueHf">BCD value</param>
		/// <returns>New structure instance</returns>
		public static BytesQuad FromBcdHighFirst(int bcdValueHf) {
			if (bcdValueHf < 0 || bcdValueHf > 99999999) throw new ArgumentOutOfRangeException(nameof(bcdValueHf), "Must be in range 0-99999999");
			int bcd = 0;
			for (int digit = 0; digit < 8; ++digit) {
				int nibble = bcdValueHf % 10;
				bcd |= nibble << (digit * 4);
				bcdValueHf /= 10;
			}
			return new BytesQuad((byte)((bcd >> 24) & 0xff), (byte)((bcd >> 16) & 0xff), (byte)((bcd >> 8) & 0xff), (byte)(bcd & 0xff));
		}

		/// <summary>
		/// Creates structure from BCD and using first byte as low
		/// </summary>
		/// <param name="bcdValueLf">BCD value</param>
		/// <returns>New structure instance</returns>
		public static BytesQuad FromBcdLowFirst(int bcdValueLf) {
			if (bcdValueLf < 0 || bcdValueLf > 99999999) throw new ArgumentOutOfRangeException(nameof(bcdValueLf), "Must be in range 0-99999999");
			int bcd = 0;
			for (int digit = 0; digit < 8; ++digit) {
				int nibble = bcdValueLf % 10;
				bcd |= nibble << (digit * 4);
				bcdValueLf /= 10;
			}
			return new BytesQuad((byte)(bcd & 0xff), (byte)((bcd >> 8) & 0xff), (byte)((bcd >> 16) & 0xff), (byte)((bcd >> 24) & 0xff));
		}
	}
}