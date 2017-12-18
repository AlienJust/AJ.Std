using System.Collections.Generic;

namespace AJ.Std.Text
{
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
	}
}