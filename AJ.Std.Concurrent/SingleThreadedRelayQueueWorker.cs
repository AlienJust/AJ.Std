using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using AJ.Std.Concurrent.Contracts;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Concurrent
{
	public sealed class SingleThreadedRelayQueueWorker<TItem> : IWorker<TItem>, IStoppableWorker {
		private readonly ILoggerWithStackTrace _debugLogger;
		private readonly object _syncUserActions;
		private readonly object _syncRunFlags;
		private readonly ConcurrentQueue<TItem> _items;
		private readonly string _name; // TODO: implement interface INamedObject
		private readonly Action<TItem> _action;
		private readonly Thread _workThread;

		private bool _isRunning;
		private bool _mustBeStopped; // Indicates worker thread to stop work (property MustBeStopped is threadsafe)

		private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;

		public SingleThreadedRelayQueueWorker(string name, Action<TItem> action, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState, ILoggerWithStackTrace debugLogger) {
			_syncRunFlags = new object();
			_syncUserActions = new object();

			_items = new ConcurrentQueue<TItem>();
			_name = name;
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));

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
					// dequeue in this cycle
					bool shouldProceed = _items.TryDequeue(out var dequeuedItem);
					if (shouldProceed) {
						try {
							_debugLogger.Log("Before user action", new StackTrace(true));
							_action(dequeuedItem);
							_debugLogger.Log("After user action", new StackTrace(true));
						}
						catch (Exception ex) {
							_debugLogger.Log(ex, new StackTrace(true));
						}
					}
					else {
						_debugLogger.Log("All actions from queue were executed, waiting for new ones", new StackTrace(true));
						_threadNotifyAboutQueueItemsCountChanged.WaitOne();

						if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
						_debugLogger.Log("MustBeStopped was false, so continue dequeuing", new StackTrace(true));

						_debugLogger.Log("New action was enqueued, or stop is required!", new StackTrace(true));
					}
				}
			}
			catch (Exception ex) {
				_debugLogger.Log(ex, new StackTrace(true));
			}
			IsRunning = false;
		}

		public void StopAsync() {
			_debugLogger.Log("Stop called", new StackTrace(true));
			lock (_syncUserActions) {
				_mustBeStopped = true;
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
		}

		public void WaitStopComplete() {
			_debugLogger.Log("Waiting for thread exit begins...", new StackTrace(true));
			while (!_workThread.Join(100)) {
				_debugLogger.Log("Waiting for thread exit...", new StackTrace(true));
				_threadNotifyAboutQueueItemsCountChanged.Set();
			}
			_debugLogger.Log("Waiting for thread exit complete", new StackTrace(true));
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
		}
	}
}