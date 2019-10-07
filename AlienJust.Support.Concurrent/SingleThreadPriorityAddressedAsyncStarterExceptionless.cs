using System;
using System.Threading;
using AlienJust.Support.Concurrent.Contracts;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Concurrent
{
	/// <summary>
	/// Runs async operations in background thread, each action has own priority and key
	/// and allows to control maximum of operations that runs same time for all keys
	/// and allows to control maximum of operations that runs same time for one key
	/// </summary>
	public sealed class SingleThreadPriorityAddressedAsyncStarterExceptionless<TAddressKey> : IPriorKeyedAsyncStarter<TAddressKey>, IStoppableWorker {
		private readonly string _name; // TODO: implement interface INamedObject
		private readonly ILogger _debugLogger;
		private readonly bool _isWaitAllTasksCompleteNeededOnStop;

		private readonly WaitableCounter _totalFlowCounter; // counter of running async actions
		private readonly SingleThreadedRelayAddressedMultiQueueWorkerExceptionless<TAddressKey, Action<IItemsReleaser<TAddressKey>>> _asyncActionQueueWorker;

		public SingleThreadPriorityAddressedAsyncStarterExceptionless(
			string name,
			ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILogger debugLogger,
			uint maxTotalFlow, uint maxFlowPerAddress, int priorityGradation, bool isWaitAllTasksCompleteNeededOnStop) {
			_name = name;
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));
			_isWaitAllTasksCompleteNeededOnStop = isWaitAllTasksCompleteNeededOnStop;

			_totalFlowCounter = new WaitableCounter(0);
			_asyncActionQueueWorker = new SingleThreadedRelayAddressedMultiQueueWorkerExceptionless<TAddressKey, Action<IItemsReleaser<TAddressKey>>>
			(
				_name, RunActionWithAsyncTailBack, threadPriority, markThreadAsBackground, apartmentState, debugLogger,
				priorityGradation,
				maxFlowPerAddress,
				maxTotalFlow);
		}

		/// <summary>
		/// Runs ending of async operation giving queue items releaser to it
		/// </summary>
		/// <param name="asyncOperationBeginAction">Action that runs at the end of async operation</param>
		/// <param name="itemsReleaser">Items releaser</param>
		private void RunActionWithAsyncTailBack(Action<IItemsReleaser<TAddressKey>> asyncOperationBeginAction, IItemsReleaser<TAddressKey> itemsReleaser) {
			try {
				_totalFlowCounter.IncrementCount();
				_debugLogger.Log("_totalFlowCounter.Count = " + _totalFlowCounter.Count);
				asyncOperationBeginAction(itemsReleaser);
			}
			catch (Exception ex) {
				_debugLogger.Log(ex);
			}
		}


		/// <summary>
		/// Adds async operations in queue
		/// </summary>
		/// <param name="asyncAction">Action that runs async</param>
		/// <param name="priority">Queue priority</param>
		/// <param name="key">Key-address</param>
		/// <returns>Id of queue item</returns>
		public Guid AddWork(Action<Action> asyncAction, int priority, TAddressKey key) {
			var id = _asyncActionQueueWorker.AddWork
			(
				key,
				itemsReleaser => asyncAction(() => {
					itemsReleaser.ReportSomeAddressedItemIsFree(key);
					_totalFlowCounter.DecrementCount();
					_debugLogger.Log("_totalFlowCounter.Count = " + _totalFlowCounter.Count);
				}),
				priority
			);
			return id;
		}

		public bool RemoveExecution(Guid id) {
			return _asyncActionQueueWorker.RemoveItem(id);
		}

		public uint MaxTotalFlow {
			// Thread safety is guaranteed by worker
			get => _asyncActionQueueWorker.MaxTotalOnetimeItemsUsages;
			set => _asyncActionQueueWorker.MaxTotalOnetimeItemsUsages = value;
		}

		public void StopAsync() {
			_asyncActionQueueWorker.StopAsync();
		}

		public void WaitStopComplete() {
			_asyncActionQueueWorker.WaitStopComplete();
			_debugLogger.Log("Background worker has been stopped            ,,,,,,,,,,,,,,");
			if (_isWaitAllTasksCompleteNeededOnStop) {
				_totalFlowCounter.WaitForCounterChangeWhileNotPredecate(count => count == 0);
				_debugLogger.Log("Total tasks count is now 0                   ..............");
			}
		}
	}
}