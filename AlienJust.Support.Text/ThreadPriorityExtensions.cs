using System.Threading;

namespace AlienJust.Support.Text
{
	public static class ThreadPriorityExtensions {
		public static string AsShortString(this ThreadPriority priority) {
			switch (priority) {
				case ThreadPriority.Normal:
					return "normal";
				case ThreadPriority.BelowNormal:
					return "below";
				case ThreadPriority.Lowest:
					return "lowest";
				case ThreadPriority.Highest:
					return "highest";
				case ThreadPriority.AboveNormal:
					return "above";
				default:
					return "unknown";
			}
		}

	}
}