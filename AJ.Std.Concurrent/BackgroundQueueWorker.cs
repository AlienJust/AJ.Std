using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using AJ.Std.Concurrent.Contracts;

namespace AJ.Std.Concurrent
{
	public sealed class BackgroundQueueWorker<TItem> : IWorker<TItem>, IThreadNotifier {
		private readonly object _sync;
		private readonly ConcurrentQueue<TItem> _items;
		private readonly Action<TItem> _actionInBackThread;
		private readonly BackgroundWorker _workThread;
		private readonly WaitableCounter _counter;

		private bool _isRunning;
		private bool _mustBeStopped;
		private readonly ManualResetEvent _endEvent;

		public BackgroundQueueWorker(Action<TItem> actionInBackThread) {
			_sync = new object();
			_endEvent = new ManualResetEvent(false);

			_items = new ConcurrentQueue<TItem>();
			_actionInBackThread = actionInBackThread;

			_counter = new WaitableCounter();

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new BackgroundWorker { WorkerReportsProgress = true };
			_workThread.DoWork += WorkingThreadStart;
			_workThread.RunWorkerAsync();
			_workThread.ProgressChanged += (sender, args) => ((Action)args.UserState).Invoke(); // if throws it will be in initial thread
		}


		public void AddWork(TItem workItem) {
			if (!MustBeStopped) {
				_items.Enqueue(workItem);
				_counter.IncrementCount();
			}
			else throw new Exception("Cannot handle items any more, asyncWorker has been stopped or stopping now");
		}


		private void WorkingThreadStart(object sender, EventArgs args) {
			try {
				while (!MustBeStopped) {
					// Waits for new items
					_counter.WaitForCounterChangeWhileNotPredecate(count => count > 0);
					while (true) {
						// dequeuing queue
						bool shouldProceed = _items.TryDequeue(out var dequeuedItem);
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
				//swallow all exceptions
			}
			finally {
				_endEvent.Set();
			}
		}

		public void Notify(Action notifyAction) {
			_workThread.ReportProgress(0, notifyAction);
		}

		public void StopSynchronously() {
			if (IsRunning) {
				MustBeStopped = true;
				_counter.IncrementCount(); //  _counter.Count is broken, but I don't care cause of thread destroying
				_endEvent.WaitOne();
				IsRunning = false;
			}
		}

		public bool IsRunning {
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

		private bool MustBeStopped {
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
}