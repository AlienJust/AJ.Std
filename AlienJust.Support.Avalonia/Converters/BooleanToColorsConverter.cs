using System;
using System.Globalization;
using System.Windows.Data;

namespace AlienJust.Support.Wpf.Converters
{
    public class BooleanToColorsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Colors))
            {
                return value != null && (bool) value ? TrueColor : FalseColor;
            }

            throw new InvalidOperationException("Converter can only convert to value of type " + typeof(Colors).FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }

        public Colors TrueColor { get; set; }
        public Colors FalseColor { get; set; }
    }
}