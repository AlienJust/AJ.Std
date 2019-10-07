using System;

namespace AlienJust.Support.Concurrent.Contracts {
	public interface IWorkerFactory
	{
		IAsyncWorker GetSimpleWorker(Action<IAsyncWorkerProgressHandler> run, Action<int> progress, Action<Exception> complete);
	}
}