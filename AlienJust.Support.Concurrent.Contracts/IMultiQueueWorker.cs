namespace AlienJust.Support.Concurrent.Contracts {
	public interface IMultiQueueWorker<in TItem>
	{
		void AddWork(TItem item, int queueNumber);
		void ClearQueue();
	}
}