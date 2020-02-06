using System;
using System.Windows.Data;
using System.Windows.Media;

namespace AlienJust.Support.Wpf.Converters
{
	[ValueConversion(typeof(Colors), typeof(Brush))]
	public class ColorsEnumToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Colors color)
			{
				return ColorsConverter.Convert(color);
			}
			throw new Exception("Wrong type: value is not of type " + typeof(Colors).FullName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
