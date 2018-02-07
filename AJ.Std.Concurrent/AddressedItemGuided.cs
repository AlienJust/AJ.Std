using System;

namespace AJ.Std.Concurrent
{
	sealed class AddressedItemGuided<TKey, TItem> {
		public TKey Key { get; }
		public TItem Item { get; }
		public Guid Id { get; }
		public AddressedItemGuided(TKey key, TItem item, Guid id) {
			Key = key;
			Item = item;
			Id = id;
		}
	}
}