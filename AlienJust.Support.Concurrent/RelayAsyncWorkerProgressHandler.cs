using System;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.Concurrent
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