namespace AlienJust.Support.Collections
{
    public static class ByteExtensions
    {
        public static int AsBcd(this byte currentByte)
        {
            int high = currentByte >> 4;
            int low = currentByte & 0xF;
            return 10 * high + low;
        }
    }
}
