using System;
using System.Windows.Media;

namespace AlienJust.Support.Wpf.Converters
{
	static class ColorsConverter
	{
		public static Color Convert(Colors color)
		{
			switch (color)
			{
				case Colors.Red:
					return System.Windows.Media.Colors.Red;
				case Colors.Blue:
					return System.Windows.Media.Colors.Blue;
				case Colors.RoyalBlue:
					return System.Windows.Media.Colors.RoyalBlue;
				case Colors.DeepSkyBlue:
					return System.Windows.Media.Colors.DeepSkyBlue;
				case Colors.LightSkyBlue:
					return System.Windows.Media.Colors.LightSkyBlue;
				case Colors.DodgerBlue:
					return System.Windows.Media.Colors.DodgerBlue;
				case Colors.SkyBlue:
					return System.Windows.Media.Colors.SkyBlue;
				case Colors.SteelBlue:
					return System.Windows.Media.Colors.SteelBlue;
				case Colors.CornflowerBlue:
					return System.Windows.Media.Colors.CornflowerBlue;
				case Colors.LightBlue:
					return System.Windows.Media.Colors.LightBlue;

				case Colors.Green:
					return System.Windows.Media.Colors.Green;

				case Colors.Black:
					return System.Windows.Media.Colors.Black;
				case Colors.Transparent:
					return System.Windows.Media.Colors.Transparent;
				case Colors.White:
					return System.Windows.Media.Colors.White;

				case Colors.Orange:
					return System.Windows.Media.Colors.Orange;
				case Colors.OrangeRed:
					return System.Windows.Media.Colors.OrangeRed;
				case Colors.Firebrick:
					return System.Windows.Media.Colors.Firebrick;

				case Colors.Gray:
					return System.Windows.Media.Colors.Gray;
				case Colors.DarkGray:
					return System.Windows.Media.Colors.DarkGray;
				case Colors.LightGray:
					return System.Windows.Media.Colors.LightGray;

				case Colors.Lime:
					return System.Windows.Media.Colors.Lime;
				case Colors.LimeGreen:
					return System.Windows.Media.Colors.LimeGreen;
				case Colors.YellowGreen:
					return System.Windows.Media.Colors.YellowGreen;
				case Colors.Yellow:
					return System.Windows.Media.Colors.Yellow;

				case Colors.PaleVioletRed:
					return System.Windows.Media.Colors.PaleVioletRed;
				case Colors.IndianRed:
					return System.Windows.Media.Colors.IndianRed;

				case Colors.Magenta:
					return System.Windows.Media.Colors.Magenta;
				case Colors.Indigo:
					return System.Windows.Media.Colors.Indigo;
				case Colors.BlueViolet:
					return System.Windows.Media.Colors.BlueViolet;

				default:
					throw new ArgumentOutOfRangeException(nameof(color));
			}
		}
	}
}