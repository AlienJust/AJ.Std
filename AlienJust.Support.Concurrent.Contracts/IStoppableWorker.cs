namespace AlienJust.Support.Concurrent.Contracts {
	public interface IStoppableWorker
	{
		void StopAsync();
		void WaitStopComplete();
	}
}