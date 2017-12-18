namespace AJ.Std.Loggers.Contracts
{
	public interface IMultiLoggerWithStackTrace<in T> {
		ILoggerWithStackTrace GetLogger(T loggerIndex);
	}
}