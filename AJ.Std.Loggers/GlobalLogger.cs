using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Loggers
{
	public sealed class GlobalLogger {
		private static ILogger _logger;
		public static void Setup(ILogger logger) {
			_logger = logger;
		}
		public static ILogger Instance => _logger;
	}
}
