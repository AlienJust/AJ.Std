using System;
using System.Collections.Generic;
using System.Linq;

namespace AlienJust.Support.Collections
{
	public static class IlistExtensions {
		public static void SerializeIntLowFirst(this IList<byte> container, int position, int value) {
			var bytes = BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray();
			BitConverter.GetBytes(value);
			container[position + 0] = bytes[0];
			container[position + 1] = bytes[1];
			container[position + 2] = bytes[2];
			container[position + 3] = bytes[3];
		}

		public static void SerializeUintLowFirst(this IList<byte> container, int position, uint value) {
			var bytes = BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray();
			BitConverter.GetBytes(value);
			container[position + 0] = bytes[0];
			container[position + 1] = bytes[1];
			container[position + 2] = bytes[2];
			container[position + 3] = bytes[3];
		}

		public static void SerializeShortLowFirst(this IList<byte> container, int position, short value) {
			var bytes = BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray();
			BitConverter.GetBytes(value);
			container[position + 0] = bytes[0];
			container[position + 1] = bytes[1];
		}

		public static void SerializeUshortLowFirst(this IList<byte> container, int position, ushort value) {
			var bytes = BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray();
			BitConverter.GetBytes(value);
			container[position + 0] = bytes[0];
			container[position + 1] = bytes[1];
		}
	}
}