using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using AJ.Std.Concurrent.Contracts;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Concurrent
{
	public sealed class AddressedConcurrentQueueWithPriority<TKey, TItem>
	{
		private readonly int _maxPriority;
		private readonly ConcurrentDictionary<TKey, ConcurrentQueueWithPriority<TItem>> _addressedQueues;
		public AddressedConcurrentQueueWithPriority(int maxPriority)
		{
			_maxPriority = maxPriority;
			_addressedQueues = new ConcurrentDictionary<TKey, ConcurrentQueueWithPriority<TItem>>();
		}

		public void Enqueue(TKey key, TItem item, int priority)
		{
			_addressedQueues.GetOrAdd(key, k => new ConcurrentQueueWithPriority<TItem>(_maxPriority)).Enqueue(item, priority);
		}

		public TItem Dequeue(TKey key)
		{
			ConcurrentQueueWithPriority<TItem> addrQueue;
			if (_addressedQueues.TryGetValue(key, out addrQueue)) {
				return addrQueue.Dequeue();
			}
			throw new Exception("Cannot get queue with priority for " + key);
		}
	}

	sealed class AddressedItem<TKey, TItem>
	{
		public TKey Key { get; private set; }
		public TItem Item { get; private set; }
		public AddressedItem(TKey key, TItem item)
		{
			Key = key;
			Item = item;
		}
	}

	sealed class AddressedItemGuided<TKey, TItem>
	{
		public TKey Key { get; private set; }
		public TItem Item { get; private set; }
		public Guid Id { get; private set; }
		public AddressedItemGuided(TKey key, TItem item, Guid id)
		{
			Key = key;
			Item = item;
			Id = id;
		}
	}

	public sealed class BackgroundQueueWorker<TItem> : IWorker<TItem>, IThreadNotifier
	{
		private readonly object _sync;
		private readonly ConcurrentQueue<TItem> _items;
		private readonly Action<TItem> _actionInBackThread;
		private readonly BackgroundWorker _workThread;
		private readonly WaitableCounter _counter;

		private bool _isRunning;
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)
		private readonly ManualResetEvent _endEvent;

		public BackgroundQueueWorker(Action<TItem> actionInBackThread)
		{
			_sync = new object();
			_endEvent = new ManualResetEvent(false);

			_items = new ConcurrentQueue<TItem>();
			_actionInBackThread = actionInBackThread;

			_counter = new WaitableCounter(); // свой счетчик с методами ожидания

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new BackgroundWorker { WorkerReportsProgress = true };
			_workThread.DoWork += WorkingThreadStart;
			_workThread.RunWorkerAsync();
			_workThread.ProgressChanged += (sender, args) => ((Action)args.UserState).Invoke(); // если вылетает исключение - то оно будет в потоке GUI
		}


		public void AddWork(TItem workItem)
		{
			if (!MustBeStopped) {
				_items.Enqueue(workItem);
				_counter.IncrementCount();
			}
			else throw new Exception("Cannot handle items any more, asyncWorker has been stopped or stopping now");
		}


		private void WorkingThreadStart(object sender, EventArgs args)
		{
			try {
				while (!MustBeStopped) {
					// В этом цикле ждем пополнения очереди:
					_counter.WaitForCounterChangeWhileNotPredecate(count => count > 0);
					while (true) {
						// в этом цикле опустошаем очередь
						TItem dequeuedItem;
						bool shouldProceed = _items.TryDequeue(out dequeuedItem);
						if (!shouldProceed) {
							break;
						}

						try {
							_actionInBackThread(dequeuedItem);
						}
						catch {
							continue;
						}
						finally {
							_counter.DecrementCount();
						}
					}
				}
			}
			catch {
				//swallow all exeptions
			}
			finally {
				_endEvent.Set();
			}
		}

		public void Notify(Action notifyAction)
		{
			_workThread.ReportProgress(0, notifyAction);
		}

		public void StopSynchronously()
		{
			if (IsRunning) {
				MustBeStopped = true;
				_counter.IncrementCount(); // счетчик очереди сбивается, но это не важно, потому что после этого метода поток уничтожается

				_endEvent.WaitOne();
				IsRunning = false;
			}
		}

		public bool IsRunning
		{
			get {
				bool result;
				lock (_sync) {
					result = _isRunning;
				}
				return result;
			}

			private set {
				lock (_sync) {
					_isRunning = value;
				}
			}
		}

		private bool MustBeStopped
		{
			get {
				bool result;
				lock (_sync) {
					result = _mustBeStopped;
				}
				return result;
			}

			set {
				lock (_sync) {
					_mustBeStopped = value;
				}
			}
		}
	}

	public sealed class ConcurrentQueueWithPriority<TItem>
	{
		private readonly List<ConcurrentQueue<TItem>> _itemsQueues;

		public ConcurrentQueueWithPriority(int maxPriority)
		{
			_itemsQueues = new List<ConcurrentQueue<TItem>>();
			for (int i = 0; i < maxPriority; ++i)
				_itemsQueues.Add(new ConcurrentQueue<TItem>());
		}


		public void Enqueue(TItem item, int priority)
		{
			_itemsQueues[priority].Enqueue(item);
		}

		/// <summary>
		/// Обходит очереди по приоритетам и выбирает элемент с наивысшим приоритетом из имеющихся
		/// </summary>
		/// <returns>Взятый из очереди элемент</returns>
		/// <exception cref="Exception">Исключение, итемов не найдено</exception>
		public TItem Dequeue()
		{
			try {
				return DequeueItemsReqursively(0);
			}
			catch (Exception ex) {
				throw new Exception("Cannot get item", ex);
			}
		}


		public bool TryDequeue(out TItem result)
		{
			return TryDequeueItemsReqursively(out result, 0);
		}

		public void ClearQueue()
		{
			foreach (var itemsQueue in _itemsQueues) {
				TItem item;
				while (itemsQueue.TryDequeue(out item)) { }
			}
		}

		private TItem DequeueItemsReqursively(int currentQueueNumber)
		{
			//GlobalLogger.Instance.Log("currentQueueNumber=" + currentQueueNumber);
			int nextQueueNumber = currentQueueNumber + 1;
			if (currentQueueNumber >= _itemsQueues.Count) throw new Exception("No more queues");

			var items = _itemsQueues[currentQueueNumber];
			TItem dequeuedItem;
			if (items.TryDequeue(out dequeuedItem)) {
				//GlobalLogger.Instance.Log("Item found, returning...");
				return dequeuedItem;
			}

			//GlobalLogger.Instance.Log("No items in queue=" + currentQueueNumber + " moving to newx queue...");
			return DequeueItemsReqursively(nextQueueNumber);
		}

		private bool TryDequeueItemsReqursively(out TItem result, int currentQueueNumber)
		{
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

	/// <summary>
	/// Очередь с приоритетом и адресацией
	/// </summary>
	/// <typeparam name="TKey">Тип адреса</typeparam>
	/// <typeparam name="TItem">Тип элемента</typeparam>
	public sealed class ConcurrentQueueWithPriorityAndAddressUsageControl<TKey, TItem>
	{
		private readonly object _syncRoot = new object();
		private readonly List<List<AddressedItem<TKey, TItem>>> _itemCollections;
		private readonly int _maxParallelUsingItemsCount;

		private readonly WaitableMultiCounter<TKey> _itemCounters;

		/// <summary>
		/// Создает новую очередь
		/// </summary>
		/// <param name="maxPriority">Максимальный приоритет</param>
		/// <param name="maxParallelUsingItemsCount">Максимальное количество одновременно разрешенных выборок элементов с одним адресом</param>
		public ConcurrentQueueWithPriorityAndAddressUsageControl(int maxPriority, int maxParallelUsingItemsCount)
		{
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
		public void ReportDecrementItemUsages(TKey address)
		{
			_itemCounters.DecrementCount(address);
		}

		/// <summary>
		/// Добавляет элемент в очередь
		/// </summary>
		/// <param name="key">Адрес элемента (ключ)</param>
		/// <param name="item">Элемент</param>
		/// <param name="priority">Приоритет (0 - наивысший приоритет)</param>
		public void Enqueue(TKey key, TItem item, int priority)
		{
			lock (_syncRoot) {
				_itemCollections[priority].Add(new AddressedItem<TKey, TItem>(key, item));
			}
		}

		/// <summary>
		/// Обходит очереди по приоритетам и выбирает элемент с наивысшим приоритетом из имеющихся
		/// </summary>
		/// <returns>Взятый из очереди элемент</returns>
		/// <exception cref="Exception">Исключение, итемов не найдено</exception>
		public TItem Dequeue()
		{
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
		private TItem DequeueItemsReqursively(int currentQueueNumber)
		{
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

		public bool TryDequeue(out TItem result)
		{
			lock (_syncRoot) {
				return TryDequeueItemsReqursively(out result, 0);
			}
		}

		private bool TryDequeueItemsReqursively(out TItem result, int currentQueueNumber)
		{
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

	/// <summary>
	/// Потокобезопасная очередь с приоритетом и адресацией
	/// </summary>
	/// <typeparam name="TKey">Тип адреса</typeparam>
	/// <typeparam name="TItem">Тип элемента</typeparam>
	public sealed class ConcurrentQueueWithPriorityAndAddressUsageControlGuided<TKey, TItem>
	{
		private readonly object _syncRoot = new object();
		private readonly List<List<AddressedItemGuided<TKey, TItem>>> _itemCollections;
		private readonly int _maxPriority;
		private readonly uint _maxParallelUsingItemsCount;
		private uint _maxTotalUsingItemsCount;
		private readonly WaitableMultiCounter<TKey> _itemsInUseCounters;

		/// <summary>
		/// Создает новую очередь
		/// </summary>
		/// <param name="maxPriority">Максимальный приоритет</param>
		/// <param name="maxParallelUsingItemsCount">Максимальное количество одновременно разрешенных выборок элементов</param>
		/// <param name="maxTotalUsingItemsCount">Максимальное общее число выборок элементов</param>
		public ConcurrentQueueWithPriorityAndAddressUsageControlGuided(int maxPriority, uint maxParallelUsingItemsCount, uint maxTotalUsingItemsCount)
		{
			_maxPriority = maxPriority;
			_maxParallelUsingItemsCount = maxParallelUsingItemsCount;
			_maxTotalUsingItemsCount = maxTotalUsingItemsCount;
			_itemCollections = new List<List<AddressedItemGuided<TKey, TItem>>>();

			for (int i = 0; i < _maxPriority; ++i) {
				_itemCollections.Add(new List<AddressedItemGuided<TKey, TItem>>());
			}

			_itemsInUseCounters = new WaitableMultiCounter<TKey>();
		}

		public uint MaxTotalUsingItemsCount
		{
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
		/// Сообщает очереди о том, что адресованный элемент больше не используется
		/// (после того как число используемых элементов снизится до разрешенного значения, будет разрешена дальнейшая выборка элементов с таким адресом)
		/// </summary>
		/// <param name="address">Адрес элемента, который больше не используется</param>
		public void ReportDecrementItemUsages(TKey address)
		{
			_itemsInUseCounters.DecrementCount(address);
		}

		/// <summary>
		/// Добавляет элемент в очередь
		/// </summary>
		/// <param name="key">Адрес элемента (ключ)</param>
		/// <param name="item">Элемент</param>
		/// <param name="priority">Приоритет (0 - наивысший приоритет)</param>
		public Guid Enqueue(TKey key, TItem item, int priority)
		{
			if (_maxPriority < priority) throw new Exception("Too low priority, must be in range non negative and less than " + _maxPriority);
			var guid = Guid.NewGuid();
			lock (_syncRoot) {
				_itemCollections[priority].Add(new AddressedItemGuided<TKey, TItem>(key, item, guid));
			}
			return guid;
		}

		/// <summary>
		/// Обходит очереди по приоритетам и выбирает элемент с наивысшим приоритетом из имеющихся
		/// </summary>
		/// <returns>Взятый из очереди элемент</returns>
		/// <exception cref="Exception">Исключение, итемов не найдено</exception>
		public TItem Dequeue()
		{
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

		private TItem DequeueItemsCycle()
		{
			// already locked:
			if (_itemsInUseCounters.TotalCount >= _maxTotalUsingItemsCount) throw new Exception("Cannot get item because max total limit riched");

			foreach (var items in _itemCollections) {
				for (int j = 0; j < items.Count; ++j) {
					var item = items[j];
					if (_itemsInUseCounters.GetCount(item.Key) < _maxParallelUsingItemsCount) // т.е. пропускаем итем в случае превышения использования итемов с таким ключем
					{
						items.RemoveAt(j);
						_itemsInUseCounters.IncrementCount(item.Key);
						return item.Item;
					}
				}
			}
			throw new Exception("All queues passed, no more queues");
		}


		public bool TryDequeue(out TItem result)
		{
			lock (_syncRoot) {
				return TryDequeueItemsCycle(out result);
			}
		}


		private bool TryDequeueItemsCycle(out TItem result)
		{
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
		/// Удаляет элемент из коллекции
		/// </summary>
		/// <param name="id">Идентификатор итема</param>
		/// <returns>Истина, если элемент с таки идентификатором был и был удален :о</returns>
		public bool RemoveItem(Guid id)
		{
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

	internal sealed class ItemReleaserRelayWithExecutionCountControl<TKey> : IItemsReleaser<TKey>
	{
		private readonly object _sync;

		private readonly IItemsReleaser<TKey> _originalItemsReleaser;
		private bool _someItemWasReleased;


		public ItemReleaserRelayWithExecutionCountControl(IItemsReleaser<TKey> originalItemsReleaser)
		{
			_sync = new object();
			_originalItemsReleaser = originalItemsReleaser;
			_someItemWasReleased = false;
		}

		public void ReportSomeAddressedItemIsFree(TKey address)
		{
			if (!SomeItemWasReleased) {
				_originalItemsReleaser.ReportSomeAddressedItemIsFree(address);
			}
			else {
				throw new Exception("This releaser cannot launch release more than once");
			}
		}

		public bool SomeItemWasReleased
		{
			get {
				bool result;
				lock (_sync) {
					result = _someItemWasReleased;
				}
				return result;
			}

			private set {
				lock (_sync) {
					_someItemWasReleased = value;
				}
			}
		}
	}

	public sealed class RelayAsyncWorker : IAsyncWorker
	{
		private readonly Action<IAsyncWorkerProgressHandler> _run;
		private readonly Action<int> _progress;
		private readonly BackgroundWorker _worker;
		private bool _wasLaunched;
		private readonly Action<Exception> _complete;
		private readonly IAsyncWorkerProgressHandler _progressChangeHandler;

		public RelayAsyncWorker(Action<IAsyncWorkerProgressHandler> run, Action<int> progress, Action<Exception> complete)
		{
			_wasLaunched = false;
			_run = run;
			_progress = progress;
			_complete = complete;


			_worker = new BackgroundWorker { WorkerReportsProgress = true };
			_worker.DoWork += (sender, args) => _run(_progressChangeHandler);
			_worker.ProgressChanged += (sender, args) => _progress(args.ProgressPercentage);
			_worker.RunWorkerCompleted += (sender, args) => _complete(args.Error);

			_progressChangeHandler = new RelayAsyncWorkerProgressHandler(p => {
				if (_worker.IsBusy)
					_worker.ReportProgress(p);
			});
		}


		public void Run()
		{
			if (_wasLaunched)
				throw new Exception("Was already launched, this worker is one-time-launch worker");
			if (_run == null)
				throw new NullReferenceException("Run action is null");
			if (_progress == null)
				throw new NullReferenceException("Progress action is null");
			if (_complete == null)
				throw new NullReferenceException("Complete action is null");

			_wasLaunched = true;
			_worker.RunWorkerAsync();
		}
	}

	public sealed class RelayAsyncWorkerProgressHandler : IAsyncWorkerProgressHandler
	{
		private readonly Action<int> _progress;
		public RelayAsyncWorkerProgressHandler(Action<int> progress)
		{
			_progress = progress;
		}
		public void NotifyProgrssChanged(int progress)
		{
			_progress?.Invoke(progress);
		}
	}

	/// <summary>
	/// Однопоточный обработчик приоритетно-адресной очереди
	/// </summary>
	/// <typeparam name="TKey">Тип адресов очереди</typeparam>
	/// <typeparam name="TItem">Тип элементов очереди</typeparam>
	public sealed class SingleThreadedRelayAddressedMultiQueueWorker<TKey, TItem> : IAddressedMultiQueueWorker<TKey, TItem>, IItemsReleaser<TKey>, IStoppableWorker {

		private readonly ConcurrentQueueWithPriorityAndAddressUsageControlGuided<TKey, TItem> _items;

		private readonly Action<TItem, IItemsReleaser<TKey>> _relayUserAction; // Пользовательское действие, которое будет совершаться над каждым элементом в порядке очереди
		private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;
		private readonly Thread _workThread;

		private readonly string _name; // TODO: implement interface INamedObject
		private readonly ILogger _debugLogger;

		private readonly object _syncRunFlags;
		private readonly object _syncUserActions;

		private bool _isRunning;
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)

		public SingleThreadedRelayAddressedMultiQueueWorker(
			string name,
			Action<TItem, IItemsReleaser<TKey>> relayUserAction,
			ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILogger debugLogger,
			int maxPriority, uint maxParallelUsingItemsCount, uint maxTotalOnetimeItemsUsages) {
			_relayUserAction = relayUserAction ?? throw new ArgumentNullException(nameof(relayUserAction));
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));

			_syncRunFlags = new object();
			_syncUserActions = new object();

			_name = name;
			
			_threadNotifyAboutQueueItemsCountChanged = new AutoResetEvent(false);
			_items = new ConcurrentQueueWithPriorityAndAddressUsageControlGuided<TKey, TItem>(maxPriority, maxParallelUsingItemsCount, maxTotalOnetimeItemsUsages);

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new Thread(WorkingThreadStart) { Priority = threadPriority, IsBackground = markThreadAsBackground, Name = name };
			if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
			_workThread.Start();
		}


		public Guid AddWork(TKey key, TItem item, int queueNumber) {
			lock (_syncUserActions) {
				lock (_syncRunFlags) {
					if (!_mustBeStopped) {
						Guid result = _items.Enqueue(key, item, queueNumber);
						_threadNotifyAboutQueueItemsCountChanged.Set();
						return result;
					}
					var ex = new Exception("Cannot handle items any more, worker has been stopped or stopping now");
					_debugLogger.Log(ex);
					throw ex;
				}
			}
		}

		public void ReportSomeAddressedItemIsFree(TKey address) {
			_items.ReportDecrementItemUsages(address);
			lock (_syncUserActions) {
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public bool RemoveItem(Guid id) {
			var result = _items.RemoveItem(id);
			lock (_syncUserActions) {
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
			return result;
		}


		private void WorkingThreadStart() {
			IsRunning = true;
			try {
				while (true) {
					try {
						var item = _items.Dequeue(); // выбрасывает исключение, если очередь пуста, и поток переходит к ожиданию сигнала
																				 //var releaser = new ItemReleaserRelayWithExecutionCountControl<TKey>((IItemsReleaser<TKey>) this);
						try {
							_relayUserAction(item, (IItemsReleaser<TKey>)this); // TODO: Warning! Если в пользовательсоком действии произойдет ошибка, то счетчик элементов застрянет!
						}
						catch (Exception ex) {
							// Даже если действие над элементом очереди не получилось, нужно проверить, не осталось ли еще чего нибудь в очереди
							// НО, я не знаю адреса:
							// if (!releaser.SomeItemWasReleased)
							//releaser.ReportSomeAddressedItemIsFree( TODO );
							_debugLogger.Log(ex);
						}
					}
					catch (Exception ex) {
						_debugLogger.Log(ex);
						_debugLogger.Log("All actions from queue were executed, waiting for new ones");
						_threadNotifyAboutQueueItemsCountChanged.WaitOne(); // Итемы кончились, начинаем ждать (основное время проводится здесь в ожидании :-))

						if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
						_debugLogger.Log("MustBeStopped was false, so continue dequeueing");

						_debugLogger.Log("New action was enqueued, or stop is required!");
					}
				}
			}
			catch (Exception ex) {
				_debugLogger.Log(ex);
			}
			IsRunning = false;
		}

		public void StopAsync() {
			_debugLogger.Log("Stop called");
			lock (_syncUserActions) {
				MustBeStopped = true;
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public void WaitStopComplete() {
			_workThread.Join();
		}

		public bool IsRunning {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _isRunning;
				}
				return result;
			}

			private set {
				lock (_syncRunFlags) {
					_isRunning = value;
				}
			}
		}

		private bool MustBeStopped {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _mustBeStopped;
				}
				return result;
			}

			set {
				lock (_syncRunFlags) {
					_mustBeStopped = value;
				}
			}
		}

		public uint MaxTotalOnetimeItemsUsages {
			// Thread safety is guaranteed by queue
			get => _items.MaxTotalUsingItemsCount;
			set {
				lock (_syncUserActions) {
					bool isDequeueNeeded = value > _items.MaxTotalUsingItemsCount;
					_items.MaxTotalUsingItemsCount = value;
					if (isDequeueNeeded) _threadNotifyAboutQueueItemsCountChanged.Set();
				}
			}
		}
	}

	/// <summary>
	/// Однопоточный обработчик приоритетно-адресной очереди
	/// </summary>
	/// <typeparam name="TKey">Тип адресов очереди</typeparam>
	/// <typeparam name="TItem">Тип элементов очереди</typeparam>
	public sealed class SingleThreadedRelayAddressedMultiQueueWorkerExceptionless<TKey, TItem> : IAddressedMultiQueueWorker<TKey, TItem>, IItemsReleaser<TKey>, IStoppableWorker {
		private readonly ConcurrentQueueWithPriorityAndAddressUsageControlGuided<TKey, TItem> _items;

		private readonly Action<TItem, IItemsReleaser<TKey>> _relayUserAction; // Пользовательское действие, которое будет совершаться над каждым элементом в порядке очереди
		private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;
		private readonly Thread _workThread;

		private readonly string _name; // TODO: implement interface INamedObject
		private readonly ILogger _debugLogger;

		private readonly object _syncRunFlags;
		private readonly object _syncUserActions;

		private bool _isRunning;
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)

		public SingleThreadedRelayAddressedMultiQueueWorkerExceptionless(
			string name,
			Action<TItem, IItemsReleaser<TKey>> relayUserAction,
			ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILogger debugLogger,
			int maxPriority, uint maxParallelUsingItemsCount, uint maxTotalOnetimeItemsUsages) {
			_syncRunFlags = new object();
			_syncUserActions = new object();

			_name = name;
			_relayUserAction = relayUserAction ?? throw new ArgumentNullException(nameof(relayUserAction));
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));

			_threadNotifyAboutQueueItemsCountChanged = new AutoResetEvent(false);
			_items = new ConcurrentQueueWithPriorityAndAddressUsageControlGuided<TKey, TItem>(maxPriority, maxParallelUsingItemsCount, maxTotalOnetimeItemsUsages);

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new Thread(WorkingThreadStart) { Priority = threadPriority, IsBackground = markThreadAsBackground, Name = name };
			if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
			_workThread.Start();
		}


		public Guid AddWork(TKey key, TItem item, int queueNumber) {
			lock (_syncUserActions) {
				lock (_syncRunFlags) {
					if (!_mustBeStopped) {
						Guid result = _items.Enqueue(key, item, queueNumber);
						_threadNotifyAboutQueueItemsCountChanged.Set();
						return result;
					}
					var ex = new Exception("Cannot handle items any more, worker has been stopped or stopping now");
					_debugLogger.Log(ex);
					throw ex;
				}
			}
		}


		public void ReportSomeAddressedItemIsFree(TKey address) {
			_items.ReportDecrementItemUsages(address);
			lock (_syncUserActions) {
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public bool RemoveItem(Guid id) {
			var result = _items.RemoveItem(id);
			lock (_syncUserActions) {
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
			return result;
		}


		private void WorkingThreadStart() {
			IsRunning = true;
			try {
				while (true) {
					bool isItemTaken = _items.TryDequeue(out var item); // выбрасывает исключение, если очередь пуста, и поток переходит к ожиданию сигнала
																													//var releaser = new ItemReleaserRelayWithExecutionCountControl<TKey>((IItemsReleaser<TKey>) this);
					if (isItemTaken) {
						try {
							_relayUserAction(item, (IItemsReleaser<TKey>)this); // TODO: Warning! Если в пользовательсоком действии произойдет ошибка, то счетчик элементов застрянет!
						}
						catch (Exception ex) {
							_debugLogger.Log(ex);
						}
					}
					else {
						_debugLogger.Log("All actions from queue were executed, waiting for new ones");
						_threadNotifyAboutQueueItemsCountChanged.WaitOne(); // Итемы кончились, начинаем ждать (основное время проводится здесь в ожидании :-))

						if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
						_debugLogger.Log("MustBeStopped was false, so continue dequeueing");

						_debugLogger.Log("New action was enqueued, or stop is required!");
					}
				}
			}
			catch (Exception ex) {
				_debugLogger.Log(ex);
			}
			IsRunning = false;
		}

		public void StopAsync() {
			_debugLogger.Log("Stop called");
			lock (_syncUserActions) {
				MustBeStopped = true;
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public void WaitStopComplete() {
			_workThread.Join();
		}

		public bool IsRunning {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _isRunning;
				}
				return result;
			}

			private set {
				lock (_syncRunFlags) {
					_isRunning = value;
				}
			}
		}

		private bool MustBeStopped {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _mustBeStopped;
				}
				return result;
			}

			set {
				lock (_syncRunFlags) {
					_mustBeStopped = value;
				}
			}
		}

		public uint MaxTotalOnetimeItemsUsages {
			// Thread safity is guaranted by queue
			get { return _items.MaxTotalUsingItemsCount; }
			set {
				lock (_syncUserActions) {
					bool isDequeueNeeded = value > _items.MaxTotalUsingItemsCount;
					_items.MaxTotalUsingItemsCount = value;
					if (isDequeueNeeded) _threadNotifyAboutQueueItemsCountChanged.Set();
				}
			}
		}
	}


	public sealed class SingleThreadedRelayMultiQueueWorker<TItem> : IMultiQueueWorker<TItem>, IStoppableWorker {
		private readonly ConcurrentQueueWithPriority<TItem> _items;
		private readonly Action<TItem> _action;
		private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;
		private readonly Thread _workThread;

		private readonly ILoggerWithStackTrace _debugLogger;
		private readonly string _name; // TODO: implement interface INamedObject
		private readonly object _syncUserActions;
		private readonly object _syncRunFlags;

		private bool _isRunning;
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)

		public SingleThreadedRelayMultiQueueWorker(string name, Action<TItem> action, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILoggerWithStackTrace debugLogger, int queuesCount) {
			_syncRunFlags = new object();
			_syncUserActions = new object();

			_name = name;
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));

			_threadNotifyAboutQueueItemsCountChanged = new AutoResetEvent(false);

			_items = new ConcurrentQueueWithPriority<TItem>(queuesCount);

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new Thread(WorkingThreadStart) { Priority = threadPriority, IsBackground = markThreadAsBackground, Name = name };
			if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
			_workThread.Start();
		}


		public void AddWork(TItem workItem, int queueNumber) {
			lock (_syncUserActions) {
				lock (_syncRunFlags) {
					if (!_mustBeStopped) {
						_items.Enqueue(workItem, queueNumber);
						_threadNotifyAboutQueueItemsCountChanged.Set();
					}
					else {
						var ex = new Exception("Cannot handle items any more, worker has been stopped or stopping now");
						_debugLogger.Log(ex, new StackTrace());
						throw ex;
					}
				}
			}
		}

		public void ClearQueue() {
			_items.ClearQueue();
		}


		private void WorkingThreadStart() {
			IsRunning = true;
			try {
				while (true) {
					try {
						var item = _items.Dequeue();
						try {
							//GlobalLogger.Instance.Log("item received, producing action on it...");
							_action(item);
						}
						catch (Exception ex) {
							_debugLogger.Log(ex, new StackTrace());
						}
					}
					catch (Exception ex) {
						_debugLogger.Log("All actions from queue were executed, waiting for new ones", new StackTrace());
						_threadNotifyAboutQueueItemsCountChanged.WaitOne(); // Итемы кончились, начинаем ждать

						if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
						_debugLogger.Log("MustBeStopped was false, so continue dequeuing", new StackTrace());

						_debugLogger.Log(ex, new StackTrace());
					}
				}
			}
			catch (Exception ex) {
				_debugLogger.Log(ex, new StackTrace());
			}
			IsRunning = false;
		}

		public void StopAsync() {
			_debugLogger.Log("Stop called", new StackTrace());
			lock (_syncUserActions) {
				_mustBeStopped = true;
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public void WaitStopComplete() {
			_workThread.Join();
		}

		public bool IsRunning {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _isRunning;
				}
				return result;
			}

			private set {
				lock (_syncRunFlags) {
					_isRunning = value;
				}
			}
		}

		private bool MustBeStopped {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _mustBeStopped;
				}
				return result;
			}

			set {
				lock (_syncRunFlags) {
					_mustBeStopped = value;
				}
			}
		}
	}

	public sealed class SingleThreadedRelayMultiQueueWorkerExceptionless<TItem> : IMultiQueueWorker<TItem>, IStoppableWorker {
		private readonly ConcurrentQueueWithPriority<TItem> _items;
		private readonly Action<TItem> _action;
		private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;
		private readonly Thread _workThread;

		private readonly string _name; // TODO: implement interface INamedObject
		private readonly ILoggerWithStackTrace _debugLogger;

		private readonly object _syncUserActions;
		private readonly object _syncRunFlags;

		private bool _isRunning;
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)


		public SingleThreadedRelayMultiQueueWorkerExceptionless(string name, Action<TItem> action, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILoggerWithStackTrace debugLogger, int queuesCount) {
			_syncRunFlags = new object();
			_syncUserActions = new object();

			_name = name;
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));

			_threadNotifyAboutQueueItemsCountChanged = new AutoResetEvent(false);

			_items = new ConcurrentQueueWithPriority<TItem>(queuesCount);

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new Thread(WorkingThreadStart) { Priority = threadPriority, IsBackground = markThreadAsBackground, Name = name };
			if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
			_workThread.Start();
		}

		public void AddWork(TItem workItem, int queueNumber) {
			lock (_syncUserActions) {
				lock (_syncRunFlags) {
					if (!_mustBeStopped) {
						_items.Enqueue(workItem, queueNumber);
						_threadNotifyAboutQueueItemsCountChanged.Set();
					}
					else {
						var ex = new Exception("Cannot handle items any more, worker has been stopped or stopping now");
						_debugLogger.Log(ex, new StackTrace());
						throw ex;
					}
				}
			}
		}

		public void ClearQueue() {
			_items.ClearQueue();
		}


		private void WorkingThreadStart() {
			IsRunning = true;
			try {
				while (true) {
					try {
						if (_items.TryDequeue(out var dequeuedItem)) {
							try {
								_action(dequeuedItem);
							}
							catch (Exception ex) {
								_debugLogger.Log(ex, new StackTrace());
							}
						}
						else {
							_debugLogger.Log("All actions from queue were executed, waiting for new ones", new StackTrace());
							_threadNotifyAboutQueueItemsCountChanged.WaitOne(); // Итемы кончились, начинаем ждать

							if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
							_debugLogger.Log("MustBeStopped was false, so continue dequeueing", new StackTrace());

							_debugLogger.Log("New action was enqueued, or stop is required!", new StackTrace());
						}
					}
					catch (Exception ex) {
						_debugLogger.Log(ex, new StackTrace());
					}
				}
			}
			catch (Exception ex) {
				_debugLogger.Log(ex, new StackTrace());
			}
			IsRunning = false;
		}

		public void StopAsync() {
			_debugLogger.Log("Stop called", new StackTrace());
			lock (_syncUserActions) {
				_mustBeStopped = true;
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public void WaitStopComplete() {
			_workThread.Join();
		}

		public bool IsRunning {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _isRunning;
				}
				return result;
			}

			private set {
				lock (_syncRunFlags) {
					_isRunning = value;
				}
			}
		}

		private bool MustBeStopped {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _mustBeStopped;
				}
				return result;
			}

			set {
				lock (_syncRunFlags) {
					_mustBeStopped = value;
				}
			}
		}
	}

	public sealed class SingleThreadedRelayQueueWorker<TItem> : IWorker<TItem>, IStoppableWorker {
		private readonly ILoggerWithStackTrace _debugLogger;
		private readonly object _syncUserActions;
		private readonly object _syncRunFlags;
		private readonly ConcurrentQueue<TItem> _items;
		private readonly string _name; // TODO: implement interface INamedObject
		private readonly Action<TItem> _action;
		private readonly Thread _workThread;
		//private readonly WaitableCounter _counter;

		private bool _isRunning;
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)

		private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;

		public SingleThreadedRelayQueueWorker(string name, Action<TItem> action, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILoggerWithStackTrace debugLogger) {
			if (action == null) throw new ArgumentNullException(nameof(action));
			if (debugLogger == null) throw new ArgumentNullException(nameof(debugLogger));

			_syncRunFlags = new object();
			_syncUserActions = new object();

			_items = new ConcurrentQueue<TItem>();
			_name = name;
			_action = action;
			_debugLogger = debugLogger;


			//_counter = new WaitableCounter(); // свой счетчик с методами ожидания
			_threadNotifyAboutQueueItemsCountChanged = new AutoResetEvent(false);

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new Thread(WorkingThreadStart) { IsBackground = markThreadAsBackground, Priority = threadPriority, Name = name };
			if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
			_workThread.Start();
		}

		public void AddWork(TItem workItem) {
			lock (_syncUserActions) {
				lock (_syncRunFlags) {
					if (!_mustBeStopped) {
						_items.Enqueue(workItem);
						_threadNotifyAboutQueueItemsCountChanged.Set();
					}
					else {
						var ex = new Exception("Cannot handle items any more, worker has been stopped or stopping now");
						_debugLogger.Log(ex, new StackTrace());
						throw ex;
					}
				}
			}
		}

		private void WorkingThreadStart() {
			IsRunning = true;
			try {
				while (true) {
					// в этом цикле опустошаем очередь
					TItem dequeuedItem;
					bool shouldProceed = _items.TryDequeue(out dequeuedItem);
					if (shouldProceed) {
						try {
							_debugLogger.Log("Before user action", new StackTrace(Thread.CurrentThread, true));
							_action(dequeuedItem);
							_debugLogger.Log("After user action", new StackTrace(Thread.CurrentThread, true));
						}
						catch (Exception ex) {
							_debugLogger.Log(ex, new StackTrace(Thread.CurrentThread, true));
						}
					}
					else {
						_debugLogger.Log("All actions from queue were executed, waiting for new ones", new StackTrace(Thread.CurrentThread, true));
						_threadNotifyAboutQueueItemsCountChanged.WaitOne();

						if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
						_debugLogger.Log("MustBeStopped was false, so continue dequeueing", new StackTrace(Thread.CurrentThread, true));

						_debugLogger.Log("New action was enqueued, or stop is required!", new StackTrace(Thread.CurrentThread, true));
					}
				}
			}
			catch (Exception ex) {
				_debugLogger.Log(ex, new StackTrace(Thread.CurrentThread, true));
			}
			IsRunning = false;
		}

		public void StopAsync() {
			_debugLogger.Log("Stop called", new StackTrace(Thread.CurrentThread, true));
			lock (_syncUserActions) {
				_mustBeStopped = true;
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public void WaitStopComplete() {
			_debugLogger.Log("Waiting for thread exit begans...", new StackTrace(Thread.CurrentThread, true));
			while (!_workThread.Join(100)) {
				_debugLogger.Log("Waiting for thread exit...", new StackTrace(Thread.CurrentThread, true));
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
			_debugLogger.Log("Waiting for thread exit complete", new StackTrace(Thread.CurrentThread, true));
		}


		public bool IsRunning {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _isRunning;
				}
				return result;
			}

			private set {
				lock (_syncRunFlags) {
					_isRunning = value;
				}
			}
		}

		private bool MustBeStopped {
			get {
				bool result;
				lock (_syncRunFlags) {
					result = _mustBeStopped;
				}
				return result;
			}

			set {
				lock (_syncRunFlags) {
					_mustBeStopped = value;
				}
			}
		}
	}
}
