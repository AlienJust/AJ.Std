using System;
using System.Windows.Data;
using System.Windows.Media;

namespace AlienJust.Support.Wpf.Converters {
	[ValueConversion(typeof(Colors), typeof(Brush))]
	public class UintToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is uint)
			{
				var color = (uint)value;
				var a = (byte)((color & 0xFF000000) >> 48);
				var r = (byte)((color & 0x00FF0000) >> 32);
				var g = (byte)((color & 0x0000FF00) >> 16);
				var b = (byte)((color & 0x000000FF));
				return new SolidColorBrush(Color.FromArgb(a, r, g, b));
			}
			throw new Exception("Wrong type: value is not uint");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}