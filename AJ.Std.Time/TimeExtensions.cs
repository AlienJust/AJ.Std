using System;

namespace AJ.Std.Time
{
	public static class TimeExtensions {
		public static DateTime RoundToLatestHalfAnHour(this DateTime time) {
			return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute < 30 ? 0 : 30, 0);
		}

		public static string ToSimpleString(this DateTime time) {
			return time.ToString("yyyy.MM.dd-HH:mm");
		}
	}
}
