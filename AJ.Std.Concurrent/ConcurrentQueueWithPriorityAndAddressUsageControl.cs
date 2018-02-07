using System;
using System.Collections.Generic;

namespace AJ.Std.Concurrent
{
	/// <summary>
	/// Очередь с приоритетом и адресацией
	/// </summary>
	/// <typeparam name="TKey">Тип адреса</typeparam>
	/// <typeparam name="TItem">Тип элемента</typeparam>
	public sealed class ConcurrentQueueWithPriorityAndAddressUsageControl<TKey, TItem> {
		private readonly object _syncRoot = new object();
		private readonly List<List<AddressedItem<TKey, TItem>>> _itemCollections;
		private readonly int _maxParallelUsingItemsCount;

		private readonly WaitableMultiCounter<TKey> _itemCounters;

		/// <summary>
		/// Создает новую очередь
		/// </summary>
		/// <param name="maxPriority">Максимальный приоритет</param>
		/// <param name="maxParallelUsingItemsCount">Максимальное количество одновременно разрешенных выборок элементов с одним адресом</param>
		public ConcurrentQueueWithPriorityAndAddressUsageControl(int maxPriority, int maxParallelUsingItemsCount) {
			_maxParallelUsingItemsCount = maxParallelUsingItemsCount;
			_itemCollections = new List<List<AddressedItem<TKey, TItem>>>();

			for (int i = 0; i < maxPriority; ++i) {
				_itemCollections.Add(new List<AddressedItem<TKey, TItem>>());
			}

			_itemCounters = new WaitableMultiCounter<TKey>();
		}

		/// <summary>
		/// Сообщяет очереди о том, что адресованный элемент больше не используется
		/// (после того как число используемых элементов снизится до разрешенного значения, будет разрешена дальнейшая выборка элементов с таким адресом)
		/// </summary>
		/// <param name="address">Адрес элемента, который больше не используется</param>
		public void ReportDecrementItemUsages(TKey address) {
			_itemCounters.DecrementCount(address);
		}

		/// <summary>
		/// Добавляет элемент в очередь
		/// </summary>
		/// <param name="key">Адрес элемента (ключ)</param>
		/// <param name="item">Элемент</param>
		/// <param name="priority">Приоритет (0 - наивысший приоритет)</param>
		public void Enqueue(TKey key, TItem item, int priority) {
			lock (_syncRoot) {
				_itemCollections[priority].Add(new AddressedItem<TKey, TItem>(key, item));
			}
		}

		/// <summary>
		/// Обходит очереди по приоритетам и выбирает элемент с наивысшим приоритетом из имеющихся
		/// </summary>
		/// <returns>Взятый из очереди элемент</returns>
		/// <exception cref="Exception">Исключение, итемов не найдено</exception>
		public TItem Dequeue() {
			try {
				lock (_syncRoot) {
					return DequeueItemsReqursively(0);
				}
			}
			catch (Exception ex) {
				throw new Exception("Cannot get item");
			}
		}

		/// <summary>
		/// Рекурсивно выбирает нужный итем
		/// </summary>
		/// <param name="currentQueueNumber">Номер очереди (приоритет)</param>
		/// <returns></returns>
		private TItem DequeueItemsReqursively(int currentQueueNumber) {
			int nextQueueNumber = currentQueueNumber + 1;
			if (currentQueueNumber >= _itemCollections.Count) throw new Exception("No more queues");

			var items = _itemCollections[currentQueueNumber];
			if (items.Count > 0) {
				for (int i = 0; i < items.Count; i++) {
					var item = items[i];
					if (_itemCounters.GetCount(item.Key) < _maxParallelUsingItemsCount) // т.е. пропускаем итем в случае превышения использования итемов с таким ключем
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
					if (_itemCounters.GetCount(item.Key) < _maxParallelUsingItemsCount) // т.е. пропускаем итем в случае превышения использования итемов с таким ключем
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