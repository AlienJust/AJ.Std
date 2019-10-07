namespace AlienJust.Support.Concurrent.Contracts {
	public interface IWorker<in TItem>
	{
		void AddWork(TItem workItem);
	}
}