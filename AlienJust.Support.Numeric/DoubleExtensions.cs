using System;

namespace AlienJust.Support.Numeric
{
	public static class DoubleExtensions {
		public static bool IsAbout(this double var, int value, double radius) {
			return Math.Abs(var - value) < radius;
		}
	}
}