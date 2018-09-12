namespace AJ.Std.Concurrent.Contracts {
	public interface IStoppableWorker
	{
		void StopAsync();
		void WaitStopComplete();
	}
}