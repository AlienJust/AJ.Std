namespace AJ.Std.Concurrent.Contracts {
	public interface IAsyncWorkerProgressHandler
	{
		void NotifyProgrssChanged(int progress);
	}
}