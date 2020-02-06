using System;
using System.Windows.Data;

namespace AlienJust.Support.Wpf.Converters {
	[ValueConversion(typeof(object), typeof(double))]
	public class DoublePercentageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double percentage;
			double.TryParse(parameter.ToString(), out percentage);

			var incomingDouble = (double)value;
			return incomingDouble * (percentage / 100);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}