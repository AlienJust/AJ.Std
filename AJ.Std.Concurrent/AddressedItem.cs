namespace AJ.Std.Concurrent
{
	sealed class AddressedItem<TKey, TItem> {
		public TKey Key { get; }
		public TItem Item { get; }
		public AddressedItem(TKey key, TItem item) {
			Key = key;
			Item = item;
		}
	}
}