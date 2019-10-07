using System;
using System.Threading;

namespace AlienJust.Support.Concurrent.Contracts {
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
}