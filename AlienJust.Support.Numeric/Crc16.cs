namespace AlienJust.Support.Numeric {
	public struct Crc16 {
		private readonly byte _low;
		private readonly byte _high;

		public Crc16(byte low, byte high)
			: this() {
			_low = low;
			_high = high;
		}

		public byte Low => _low;

		public byte High => _high;

		public override string ToString() {
			return "L: 0x" + _low.ToString("X2") + ", H: 0x" + _high.ToString("X2");
		}
	}
}
