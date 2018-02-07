using System;
using System.Collections.Generic;

namespace AJ.Std.Concurrent
{
	/// <summary>
	/// Queue with priority and addresses
	/// </summary>
	/// <typeparam name="TKey">Element address type</typeparam>
	/// <typeparam name="TItem">Element value type</typeparam>
	public sealed class ConcurrentQueueWithPriorityAndAddressUsageControl<TKey, TItem> {
		private readonly object _syncRoot = new object();
		private readonly List<List<AddressedItem<TKey, TItem>>> _itemCollections;
		private readonly int _maxParallelUsingItemsCount;

		private readonly WaitableMultiCounter<TKey> _itemCounters;

		/// <summary>
		/// Creates new queue
		/// </summary>
		/// <param name="maxPriority">Maximum priority</param>
		/// <param name="maxParallelUsingItemsCount">Maximum allowed items to dequeue with same address value</param>
		public ConcurrentQueueWithPriorityAndAddressUsageControl(int maxPriority, int maxParallelUsingItemsCount) {
			_maxParallelUsingItemsCount = maxParallelUsingItemsCount;
			_itemCollections = new List<List<AddressedItem<TKey, TItem>>>();

			for (int i = 0; i < maxPriority; ++i) {
				_itemCollections.Add(new List<AddressedItem<TKey, TItem>>());
			}

			_itemCounters = new WaitableMultiCounter<TKey>();
		}

		/// <summary>
		/// Says to queue that item of some address is not in use anymore
		/// </summary>
		/// <param name="address">Address of element that not in use anymore</param>
		public void ReportDecrementItemUsages(TKey address) {
			_itemCounters.DecrementCount(address);
		}

		/// <summary>
		/// Adds element to queue
		/// </summary>
		/// <param name="key">Address of the element</param>
		/// <param name="item">Value of the element</param>
		/// <param name="priority">Priority or queue number (0 - is highest)</param>
		public void Enqueue(TKey key, TItem item, int priority) {
			lock (_syncRoot) {
				_itemCollections[priority].Add(new AddressedItem<TKey, TItem>(key, item));
			}
		}

		/// <summary>
		/// Dequeue item (with highest priority first)
		/// </summary>
		/// <returns>Taken element</returns>
		/// <exception cref="Exception">No elements found</exception>
		public TItem Dequeue() {
			try {
				lock (_syncRoot) {
					return DequeueItemsReqursively(0);
				}
			}
			catch (Exception ex) {
				throw new Exception("Cannot get item", ex);
			}
		}

		/// <summary>
		/// Recurse search for an element to take
		/// </summary>
		/// <param name="currentQueueNumber">Queue number (priority)</param>
		/// <returns></returns>
		private TItem DequeueItemsReqursively(int currentQueueNumber) {
			int nextQueueNumber = currentQueueNumber + 1;
			if (currentQueueNumber >= _itemCollections.Count) throw new Exception("No more queues");

			var items = _itemCollections[currentQueueNumber];
			if (items.Count > 0) {
				for (int i = 0; i < items.Count; i++) {
					var item = items[i];
					if (_itemCounters.GetCount(item.Key) < _maxParallelUsingItemsCount) // we skip element if usage of such address if exceeded allowed limit
					{
						items.RemoveAt(i);
						_itemCounters.IncrementCount(item.Key);
						return item.Item;
					}
				}
			}
			return DequeueItemsReqursively(nextQueueNumber);
		}

		public bool TryDequeue(out TItem result) {
			lock (_syncRoot) {
				return TryDequeueItemsReqursively(out result, 0);
			}
		}

		private bool TryDequeueItemsReqursively(out TItem result, int currentQueueNumber) {
			int nextQueueNumber = currentQueueNumber + 1;
			if (currentQueueNumber >= _itemCollections.Count) throw new Exception("No more queues");

			var items = _itemCollections[currentQueueNumber];
			if (items.Count > 0) {
				for (int i = 0; i < items.Count; i++) {
					var item = items[i];
					if (_itemCounters.GetCount(item.Key) < _maxParallelUsingItemsCount)
					{
						items.RemoveAt(i);
						_itemCounters.IncrementCount(item.Key);
						result = item.Item;
						return true;
					}
				}
			}
			return TryDequeueItemsReqursively(out result, nextQueueNumber);
		}
	}
}