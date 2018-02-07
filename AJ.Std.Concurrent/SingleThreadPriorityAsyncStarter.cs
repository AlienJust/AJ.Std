using System;
using System.Diagnostics;
using System.Threading;
using AJ.Std.Concurrent.Contracts;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Concurrent
{
	/// <summary>
	/// Runs async operations in background thread, each action has own priority and key
	/// and allows to control maximum of operations that runs same time
	/// </summary>
	public sealed class SingleThreadPriorityAsyncStarter : IStoppableWorker {
		private readonly int _maxFlow;
		private readonly bool _isWaitAllTasksCompleteNeededOnStop;
		private readonly string _name; // TODO: implement interface INamedObject
		private readonly ILoggerWithStackTrace _debugLogger;
		private readonly SingleThreadedRelayMultiQueueWorker<Action> _asyncActionQueueWorker;
		private readonly WaitableCounter _flowCounter;

		public SingleThreadPriorityAsyncStarter(string name, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILoggerWithStackTrace debugLogger, int maxFlow, int queuesCount, bool isWaitAllTasksCompleteNeededOnStop) {
			_name = name;
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));
			_maxFlow = maxFlow;
			_isWaitAllTasksCompleteNeededOnStop = isWaitAllTasksCompleteNeededOnStop;

			_flowCounter = new WaitableCounter();
			_asyncActionQueueWorker = new SingleThreadedRelayMultiQueueWorker<Action>(_name, a => a(), threadPriority, markThreadAsBackground, apartmentState, debugLogger, queuesCount);
		}


		/// <summary>
		/// Adds async operations in queue
		/// </summary>
		/// <param name="asyncAction">Action that runs async</param>
		/// <param name="queueNumber">Queue priority</param>
		public void AddWork(Action asyncAction, int queueNumber) {
			_asyncActionQueueWorker.AddWork
			(
				() => {
					_flowCounter.WaitForCounterChangeWhileNotPredecate(curCount => curCount < _maxFlow);
					_flowCounter.IncrementCount();
					_debugLogger.Log("_flowCounter.Count = " + _flowCounter.Count, new StackTrace());
					asyncAction();
				},
				queueNumber
			);
		}

		/// <summary>
		/// Calls by client and notifies starter about async operation is complete
		/// </summary>
		public void NotifyStarterAboutQueuedOperationComplete() {
			_flowCounter.DecrementCount();
			_debugLogger.Log("_flowCounter.Count = " + _flowCounter.Count, new StackTrace());
		}

		public void StopAsync() {
			_asyncActionQueueWorker.StopAsync();
		}

		public void WaitStopComplete() {
			_asyncActionQueueWorker.WaitStopComplete();
			_debugLogger.Log("Background worker has been stopped            ,,,,,,,,,,,,,,", new StackTrace());
			if (_isWaitAllTasksCompleteNeededOnStop) {
				_flowCounter.WaitForCounterChangeWhileNotPredecate(count => count == 0);
				_debugLogger.Log("Total tasks count is now 0                   ..............", new StackTrace());
			}
		}
	}
}