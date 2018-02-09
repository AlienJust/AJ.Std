using System;

namespace AJ.Std.DotNetExtensions
{
	public static class FunctionalExt {
		public static void CheckCondition(this Func<bool> condition, Action onTrue, Action onFalse) {
			if (condition())
				onTrue();
			else
				onFalse();
		}
	}
}