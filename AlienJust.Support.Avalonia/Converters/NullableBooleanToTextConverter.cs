using System;
using System.Globalization;
using System.Windows.Data;

namespace AlienJust.Support.Wpf.Converters {
    public class NullableBooleanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                if (value != null)
                {
                    return (bool) value ? TrueText : FalseText;
                }

                return NullText;
            }

            throw new InvalidOperationException("Converter can only convert to value of type " + typeof(string).FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }

        public string TrueText { get; set; }
        public string FalseText { get; set; }
        public string NullText { get; set; }
    }
}