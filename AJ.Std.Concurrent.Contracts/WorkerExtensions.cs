using System;
using System.Threading;

namespace AJ.Std.Concurrent.Contracts {
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