using System;
using AJ.Std.Concurrent.Contracts;

namespace AJ.Std.Concurrent
{
	public sealed class RelayAsyncWorkerProgressHandler : IAsyncWorkerProgressHandler {
		private readonly Action<int> _progress;
		public RelayAsyncWorkerProgressHandler(Action<int> progress) {
			_progress = progress;
		}
		public void NotifyProgrssChanged(int progress) {
			_progress?.Invoke(progress);
		}
	}
}