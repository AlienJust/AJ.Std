using System;

namespace AJ.Std.DotNetExtensions {
	public static class ObjectExt {
		public static bool IsNotNullAndPredecate(this object obj, Func<bool> predecate) {
			return obj != null && predecate();
		}
	}
}
