using System;

namespace AlienJust.Support.Collections
{
    public static class ByteExtensions
    {
        /// <summary>
        /// For example takes 0x60 and makes 60 (0x3C)
        /// </summary>
        /// <param name="currentByte">byte to parse</param>
        /// <returns></returns>
        public static int AsBcd(this byte currentByte)
        {
            int high = currentByte >> 4;
            int low = currentByte & 0xF;
            return 10 * high + low;
        }

        /// <summary>
        /// For example takes 60 and makes 96 (0x60)
        /// </summary>
        /// <param name="currentByte">byte to parse</param>
        /// <returns></returns>
        public static byte ToBcd(this byte currentByte)
        {
            if (currentByte > 99) throw new ArgumentOutOfRangeException(nameof(currentByte), "Input value must be from 0 to 99");
            
            var low = currentByte % 10;
            var high = currentByte / 10;
            return (byte)((high << 4) | low);
        }
    }
}
