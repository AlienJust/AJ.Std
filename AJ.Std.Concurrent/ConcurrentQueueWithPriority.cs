using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AJ.Std.Concurrent
{
	public sealed class ConcurrentQueueWithPriority<TItem> {
		private readonly List<ConcurrentQueue<TItem>> _itemsQueues;

		public ConcurrentQueueWithPriority(int maxPriority) {
			_itemsQueues = new List<ConcurrentQueue<TItem>>();
			for (int i = 0; i < maxPriority; ++i)
				_itemsQueues.Add(new ConcurrentQueue<TItem>());
		}


		public void Enqueue(TItem item, int priority) {
			_itemsQueues[priority].Enqueue(item);
		}

		/// <summary>
		/// Dequeues item (takes item with higher priority)
		/// </summary>
		/// <returns>Taken item</returns>
		/// <exception cref="Exception">If no items found</exception>
		public TItem Dequeue() {
			try {
				return DequeueItemsReqursively(0);
			}
			catch (Exception ex) {
				throw new Exception("Cannot get item", ex);
			}
		}


		public bool TryDequeue(out TItem result) {
			return TryDequeueItemsReqursively(out result, 0);
		}

		public void ClearQueue() {
			foreach (var itemsQueue in _itemsQueues) {
				TItem item;
				while (itemsQueue.TryDequeue(out item)) { }
			}
		}

		private TItem DequeueItemsReqursively(int currentQueueNumber) {
			//GlobalLogger.Instance.Log("currentQueueNumber=" + currentQueueNumber);
			int nextQueueNumber = currentQueueNumber + 1;
			if (currentQueueNumber >= _itemsQueues.Count) throw new Exception("No more queues");

			var items = _itemsQueues[currentQueueNumber];
			if (items.TryDequeue(out var dequeuedItem)) {
				//GlobalLogger.Instance.Log("Item found, returning...");
				return dequeuedItem;
			}

			//GlobalLogger.Instance.Log("No items in queue=" + currentQueueNumber + " moving to newx queue...");
			return DequeueItemsReqursively(nextQueueNumber);
		}

		private bool TryDequeueItemsReqursively(out TItem result, int currentQueueNumber) {
			int nextQueueNumber = currentQueueNumber + 1;
			if (currentQueueNumber >= _itemsQueues.Count) {
				result = default(TItem);
				return false;
			}

			var items = _itemsQueues[currentQueueNumber];
			if (items.TryDequeue(out result)) {
				return true;
			}

			return TryDequeueItemsReqursively(out result, nextQueueNumber);
		}
	}
}