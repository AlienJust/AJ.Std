using System;
using System.Windows.Data;

namespace AlienJust.Support.Wpf.Converters {
	[ValueConversion(typeof(object), typeof(double))]
	public class DoubleAdditiveConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double addition;
			double.TryParse(parameter.ToString(), out addition);

			var incomingDouble = (double)value;
			return incomingDouble + addition;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}