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
		private bool _mustBeStopped; // Флаг, подающий фоновому потоку сигнал о необходимости завершения (обращение идет через потокобезопасное свойство MustBeStopped)
		private readonly ManualResetEvent _endEvent;

		public BackgroundQueueWorker(Action<TItem> actionInBackThread) {
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

		public void Notify(Action notifyAction) {
			_workThread.ReportProgress(0, notifyAction);
		}

		public void StopSynchronously() {
			if (IsRunning) {
				MustBeStopped = true;
				_counter.IncrementCount(); // счетчик очереди сбивается, но это не важно, потому что после этого метода поток уничтожается

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