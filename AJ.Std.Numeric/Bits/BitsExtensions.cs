using System;
using System.Collections.Generic;
using System.Text;

namespace AJ.Std.Numeric.Bits
{
	public static class BitsExtensions {
		public static bool GetBit(this byte b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this sbyte b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this short b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this ushort b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this int b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this uint b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this long b, int bitNumber) {
			return (b & 1 << bitNumber) != 0;
		}

		public static bool GetBit(this ulong b, int bitNumber) {
			return (b & (ulong)1 << bitNumber) != 0;
		}



		public static byte SetBit(this byte b, int bitNumber) {
			return (byte)(b | 1 << bitNumber);
		}

		public static sbyte SetBit(this sbyte b, int bitNumber) {
			return (sbyte)(b | 1 << bitNumber);
		}

		public static ushort SetBit(this ushort b, int bitNumber) {
			return (ushort)(b | 1 << bitNumber);
		}

		public static short SetBit(this short b, int bitNumber) {
			return (short)(b | 1 << bitNumber);
		}

		public static uint SetBit(this uint b, int bitNumber) {
			return b | (uint)1 << bitNumber;
		}

		public static int SetBit(this int b, int bitNumber) {
			return b | 1 << bitNumber;
		}

		public static long SetBit(this long b, int bitNumber) {
			return b | (long)1 << bitNumber;
		}

		public static ulong SetBit(this ulong b, int bitNumber) {
			return b | (ulong)1 << bitNumber;
		}


		public static byte ResetBit(this byte b, int bitNumber) {

			return (byte)(b & ~(1 << bitNumber));
		}

		public static sbyte ResetBit(this sbyte b, int bitNumber) {
			return (sbyte)(b & ~(1 << bitNumber));
		}

		public static ushort ResetBit(this ushort b, int bitNumber) {
			return (ushort)(b & ~(1 << bitNumber));
		}

		public static short ResetBit(this short b, int bitNumber) {
			return (short)(b & ~(1 << bitNumber));
		}

		public static uint ResetBit(this uint b, int bitNumber) {
			return b & ~((uint)1 << bitNumber);
		}

		public static int ResetBit(this int b, int bitNumber) {
			return b & ~(1 << bitNumber);
		}

		public static long ResetBit(this long b, int bitNumber) {
			return b & ~((long)1 << bitNumber);
		}

		public static ulong ResetBit(this ulong b, int bitNumber) {
			return b & ~((ulong)1 << bitNumber);
		}
	}
}
