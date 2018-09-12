namespace AJ.Std.Concurrent.Contracts {
	public interface IWorker<in TItem>
	{
		void AddWork(TItem workItem);
	}
}