namespace AJ.Std.Collections
{
	public static class ByteExtensions {
		public static int ToBcdInteger(this byte currentByte) {
			int high = currentByte >> 4;
			int low = currentByte & 0xF;
			return 10 * high + low;
		}
	}
}
