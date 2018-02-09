namespace AJ.Std.DotNetExtensions
{
	public static class NullabelBoolExt {
		public static bool IsNotNullAndTrue(this bool? check) {
			return check.HasValue && check.Value;
		}

		public static bool IsNotNullAndFalse(this bool? check) {
			return check.HasValue && !check.Value;
		}

		public static bool IsNullOrFalse(this bool? check) {
			return !check.HasValue || check.Value == false;
		}
	}
}