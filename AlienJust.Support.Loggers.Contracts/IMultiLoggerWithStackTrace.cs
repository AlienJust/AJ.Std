namespace AlienJust.Support.Loggers.Contracts
{
	public interface IMultiLoggerWithStackTrace<in T> {
		ILoggerWithStackTrace GetLogger(T loggerIndex);
	}
}