using System;
using System.Collections.Generic;

namespace AlienJust.Support.Concurrent
{
	/// <summary>
	/// Threadsafe queue of items with priority and addresses with usage count control per address
	/// </summary>
	/// <typeparam name="TKey">Item address (key) type</typeparam>
	/// <typeparam name="TItem">Item value type</typeparam>
	public sealed class ConcurrentQueueWithPriorityAndAddressUsageControlGuided<TKey, TItem> {
		private readonly object _syncRoot = new object();
		private readonly List<List<AddressedItemGuided<TKey, TItem>>> _itemCollections;
		private readonly int _maxPriority;
		private readonly uint _maxParallelUsingItemsCount;
		private uint _maxTotalUsingItemsCount;
		private readonly WaitableMultiCounter<TKey> _itemsInUseCounters;

		/// <summary>
		/// Creates new queue
		/// </summary>
		/// <param name="maxPriority">Maximum priority gradation (priorities are: 0 - maxPriority)</param>
		/// <param name="maxParallelUsingItemsCount">Maximum allowed count of dequeued elements same time</param>
		/// <param name="maxTotalUsingItemsCount">Maximum allowed count of dequeued elements</param>
		public ConcurrentQueueWithPriorityAndAddressUsageControlGuided(int maxPriority, uint maxParallelUsingItemsCount, uint maxTotalUsingItemsCount) {
			_maxPriority = maxPriority;
			_maxParallelUsingItemsCount = maxParallelUsingItemsCount;
			_maxTotalUsingItemsCount = maxTotalUsingItemsCount;
			_itemCollections = new List<List<AddressedItemGuided<TKey, TItem>>>();

			for (int i = 0; i < _maxPriority; ++i) {
				_itemCollections.Add(new List<AddressedItemGuided<TKey, TItem>>());
			}

			_itemsInUseCounters = new WaitableMultiCounter<TKey>();
		}

		public uint MaxTotalUsingItemsCount {
			get {
				lock (_syncRoot)
					return _maxTotalUsingItemsCount;
			}
			set {
				lock (_syncRoot) {
					_maxTotalUsingItemsCount = value;
				}
			}
		}

		/// <summary>
		/// Says to this queue that addressed element is not in use anymore
		/// (after the count of used elements became less then allowed one, it will be allowed to use element with such address)
		/// </summary>
		/// <param name="address">Address of element that not in use anymore</param>
		public void ReportDecrementItemUsages(TKey address) {
			_itemsInUseCounters.DecrementCount(address);
		}

		/// <summary>
		/// Enqueues an element
		/// </summary>
		/// <param name="key">Address of element (key)</param>
		/// <param name="item">Element's value</param>
		/// <param name="priority">Priority for dequeuing (0 - is the highest priority)</param>
		/// <returns>Id of the element in queue</returns>
		public Guid Enqueue(TKey key, TItem item, int priority) {
			if (_maxPriority < priority) throw new Exception("Too low priority, must be in range 0 - " + _maxPriority);
			var guid = Guid.NewGuid();
			lock (_syncRoot) {
				_itemCollections[priority].Add(new AddressedItemGuided<TKey, TItem>(key, item, guid));
			}
			return guid;
		}

		/// <summary>
		/// Dequeues next element (high priority are dequeued first)
		/// </summary>
		/// <returns>Element, that was taken from queue</returns>
		/// <exception cref="Exception">No element was found</exception>
		public TItem Dequeue() {
			try {
				TItem result;
				lock (_syncRoot) {
					result = DequeueItemsCycle();
				}
				return result;
			}
			catch (Exception ex) {
				throw new Exception("Cannot get item", ex);
			}
		}

		private TItem DequeueItemsCycle() {
			// already locked:
			if (_itemsInUseCounters.TotalCount >= _maxTotalUsingItemsCount) throw new Exception("Cannot get item because max total limit exceeded");

			foreach (var items in _itemCollections) {
				for (int j = 0; j < items.Count; ++j) {
					var item = items[j];
					if (_itemsInUseCounters.GetCount(item.Key) < _maxParallelUsingItemsCount) // skip element if usage count limit exceeded
					{
						items.RemoveAt(j);
						_itemsInUseCounters.IncrementCount(item.Key);
						return item.Item;
					}
				}
			}
			throw new Exception("All queues passed, no more queues");
		}


		public bool TryDequeue(out TItem result) {
			lock (_syncRoot) {
				return TryDequeueItemsCycle(out result);
			}
		}


		private bool TryDequeueItemsCycle(out TItem result) {
			if (_itemsInUseCounters.TotalCount >= _maxTotalUsingItemsCount) {
				result = default(TItem);
				return false;
			}


			foreach (var items in _itemCollections) {
				for (int j = 0; j < items.Count; ++j) {
					var item = items[j];
					if (_itemsInUseCounters.GetCount(item.Key) < _maxParallelUsingItemsCount) // т.е. пропускаем итем в случае превышения использования итемов с таким ключем
					{
						items.RemoveAt(j);
						_itemsInUseCounters.IncrementCount(item.Key);
						result = item.Item;
						return true;
					}
				}
			}
			result = default(TItem);
			return false;
		}


		/// <summary>
		/// Removes element from queue
		/// </summary>
		/// <param name="id">Item's identifier</param>
		/// <returns>True if element was deleted</returns>
		public bool RemoveItem(Guid id) {
			var result = false;
			lock (_syncRoot) {
				List<AddressedItemGuided<TKey, TItem>> foundCollection = null;
				AddressedItemGuided<TKey, TItem> foundItem = null;
				foreach (var collection in _itemCollections) {
					foreach (var item in collection) {
						if (item.Id == id) {
							foundItem = item;
							break;
						}
					}
					if (foundItem != null) {
						foundCollection = collection;
						break;
					}
				}
				// if collection is not null, then found item is always not null:
				if (foundCollection != null) {
					result = foundCollection.Remove(foundItem);
				}
			}
			return result;
		}
	}
}