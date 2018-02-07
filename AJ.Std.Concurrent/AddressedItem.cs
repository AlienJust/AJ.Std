namespace AJ.Std.Concurrent
{
	sealed class AddressedItem<TKey, TItem> {
		public TKey Key { get; private set; }
		public TItem Item { get; private set; }
		public AddressedItem(TKey key, TItem item) {
			Key = key;
			Item = item;
		}
	}
}