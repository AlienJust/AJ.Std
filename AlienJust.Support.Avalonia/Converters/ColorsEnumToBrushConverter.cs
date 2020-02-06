using System;
using System.Windows.Data;
using System.Windows.Media;

namespace AlienJust.Support.Wpf.Converters {
	[ValueConversion(typeof(Colors), typeof(Brush))]
	public class ColorsEnumToBrushConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is Colors color) {
				return new SolidColorBrush(ColorsConverter.Convert(color));
			}
			throw new Exception("Wrong type: value is not Colors");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
