using System;

namespace AJ.Std.Concurrent.Contracts {
	public interface IWorkerFactory
	{
		IAsyncWorker GetSimpleWorker(Action<IAsyncWorkerProgressHandler> run, Action<int> progress, Action<Exception> complete);
	}
}