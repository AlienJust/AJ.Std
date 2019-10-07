using System.Collections.Generic;

namespace AlienJust.Support.Text {
	public static class EnumerableExtensions {
		public static string ToText(this IEnumerable<byte> data) {
			int count = 0;
			string result = string.Empty;
			foreach (byte b in data) {
				result += b.ToString("X2") + " ";
				count++;
			}
			result += "[" + count + "]";
			return result;
		}

		public static string ToHexadecimalTextWithoutLength(this IEnumerable<byte> data) {
			string result = string.Empty;
			foreach (byte b in data) {
				result += b.ToString("X2") + " ";
			}
			return result;
		}

		public static string ToTextDecimalWithLength(this IEnumerable<byte> data) {
			int count = 0;
			string result = string.Empty;
			foreach (byte b in data) {
				result += b + " ";
				count++;
			}
			result += "[" + count + "]";
			return result;
		}

		public static string ToTextDecimalWithoutLength(this IEnumerable<byte> data) {
			string result = string.Empty;
			foreach (byte b in data) {
				result += b + " ";
			}
			return result;
		}
	}
}