using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AlienJust.Support.Wpf.Converters {
	public class BooleanToVisibilityConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType == typeof(Visibility))
			{
				var visible = System.Convert.ToBoolean(value, culture);
				if (InvertVisibility)
					visible = !visible;
				return visible ? Visibility.Visible : Visibility.Collapsed;
			}
			throw new InvalidOperationException("Converter can only convert to value of type " + typeof(Visibility).FullName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("Converter cannot convert back.");
		}

		public bool InvertVisibility { get; set; }

	}
}