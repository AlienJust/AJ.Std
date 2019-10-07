namespace AlienJust.Support.Concurrent.Contracts {
	public interface IAsyncWorkerProgressHandler
	{
		void NotifyProgrssChanged(int progress);
	}
}