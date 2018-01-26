using System;
using System.Threading;

namespace AJ.Std.Concurrent.Contracts
{
	/// <summary>
	/// Worker that uses queue with priority
	/// </summary>
	/// <typeparam name="TKey">Queue key type</typeparam>
	/// <typeparam name="TItem">Queue value type</typeparam>
	public interface IAddressedMultiQueueWorker<in TKey, in TItem>
	{
		/// <summary>
		/// Adds data with needed address and with needed priority in queue
		/// </summary>
		/// <param name="address">Address of element</param>
		/// <param name="item">Value of element</param>
		/// <param name="queueNumber">Number of queue (usually using as priority)</param>
		/// <returns>Unique identifier of the element in queue</returns>
		Guid AddWork(TKey address, TItem item, int queueNumber);

		/// <summary>
		/// Removes element from queue
		/// </summary>
		/// <param name="id">Unique identifier of the removing element</param>
		/// <returns>True if removed</returns>
		bool RemoveItem(Guid id);
	}

	public interface IAsyncWorker
	{
		void Run();
	}

	public interface IAsyncWorkerProgressHandler
	{
		void NotifyProgrssChanged(int progress);
	}

	public interface IGuidMemory<in T>
	{
		Guid AddObject(T obj);
		void AddObject(Guid guid, T obj);
		void RemoveObject(Guid guid);
	}

	/// <summary>
	/// Marks elements as free
	/// </summary>
	/// <typeparam name="TKey">Address type of elements</typeparam>
	public interface IItemsReleaser<in TKey>
	{

		/// <summary>
		/// Frees elements at the given address
		/// </summary>
		/// <param name="address">Address of the element that would be marked as free</param>
		void ReportSomeAddressedItemIsFree(TKey address);
	}

	public interface IMultiQueueWorker<in TItem>
	{
		void AddWork(TItem item, int queueNumber);
		void ClearQueue();
	}

	public interface IPriorKeyedAsyncStarter<in TAddressKey>
	{
		Guid AddWork(Action<Action> asyncAction, int priority, TAddressKey key);
		bool RemoveExecution(Guid id);
	}

	public interface IStoppableWorker
	{
		void StopAsync();
		void WaitStopComplete();
	}

	public interface IThreadNotifier
	{
		void Notify(Action notifyAction);
	}

	public interface IWorker<in TItem>
	{
		void AddWork(TItem workItem);
	}

	public interface IWorkerFactory
	{
		IAsyncWorker GetSimpleWorker(Action<IAsyncWorkerProgressHandler> run, Action<int> progress, Action<Exception> complete);
	}

	public static class ThreadNotifierExtensions
	{
		public static void NotifyAndWait(this IThreadNotifier tn, Action notifyAction)
		{
			var signal = new AutoResetEvent(false);
			Exception executionExcpetion = null;
			tn.Notify(() => {
				try {
					notifyAction();
				}
				catch (Exception ex) {
					executionExcpetion = ex;
				}

				signal.Set();
			});
			signal.WaitOne();
			if (executionExcpetion != null) throw new Exception("Exception during notification", executionExcpetion);
		}
	}

	public static class WorkerExtensions
	{
		public static void AddToQueueAndWaitExecution(this IWorker<Action> asyncWorker, Action a)
		{
			var signal = new ManualResetEvent(false);
			Exception exception = null;
			asyncWorker.AddWork(() => {
				try {
					a();
				}
				catch (Exception ex) {
					exception = ex;
				}
				finally {
					signal.Set();
				}
			});
			signal.WaitOne();
			if (exception != null) throw exception;
		}

		public static void AddToQueueAndWaitExecution(this IWorker<Action> asyncWorker, Action a, TimeSpan timeout)
		{
			var sync = new object();
			var signal = new ManualResetEvent(false);
			bool wasExecuted = false;
			Exception exception = null;
			asyncWorker.AddWork(() => {
				try {
					a();
					lock (sync) {
						wasExecuted = true;
					}
				}
				catch (Exception ex) {
					exception = ex;
				}
				finally {
					signal.Set();
				}
			});
			signal.WaitOne(timeout);
			bool hasBeenExecuted;
			lock (sync) {
				hasBeenExecuted = wasExecuted;
			}
			if (!hasBeenExecuted) throw new Exception("Таймаут операции");
			if (exception != null) throw exception;
		}
	}
}