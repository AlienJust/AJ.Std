using System;
using System.Threading;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.Concurrent {
	/// <summary>
	/// Worker that drops items adding if already working with other one
	/// </summary>
	/// <typeparam name="TItem">Worker item type</typeparam>
	public sealed class WorkerSingleThreadedRelayDrop<TItem> : IWorker<TItem> {
		private readonly object _sync;
		private readonly Thread _workThread;
		private readonly ManualResetEvent _signal;

		private readonly Action<TItem> _action;


		private bool _isInProgress;
		private bool _isRunning;
		private bool _mustBeStopped;
		private TItem _item;


		public WorkerSingleThreadedRelayDrop(Action<TItem> action, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState) {
			_sync = new object();
			_action = action;

			_signal = new ManualResetEvent(false);

			_isRunning = true;
			_mustBeStopped = false;

			_workThread = new Thread(WorkingThreadStart) { IsBackground = markThreadAsBackground, Priority = threadPriority };
			if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
			_workThread.Start();
		}


		public void AddWork(TItem workItem) {
			lock (_sync) {
				if (!_mustBeStopped && !_isInProgress) {
					_item = workItem;
					_isInProgress = true;
					_signal.Set();
				}
				else throw new Exception("Cannot handle items any more, asyncWorker has been stopped or stopping now");
			}
		}

		private void WorkingThreadStart() {
			try {
				while (!MustBeStopped) {
					_signal.WaitOne();

					try {
						TItem item;
						lock (_sync) item = _item;
						_action(item);
					}
					catch {
						continue;
					}
					finally {
						lock (_sync) {
							_signal.Reset();
							_isInProgress = false;
						}
					}
				}
			}
			catch {
				//swallow all exceptions
			}
		}

		public void StopSynchronously() {
			if (IsRunning) {
				lock (_sync) {
					_mustBeStopped = true;
					_signal.Set();
				}

				_workThread.Join();
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
		}
	}
}
