using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AlienJust.Support.Wpf.Converters {
    public sealed class NullableBooleanToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (targetType == typeof(Brush))
            {
                if (value != null)
                {
                    return (bool) value ? TrueBrush : FalseBrush;
                }
                return NullBrush;
            }
            throw new InvalidOperationException("Converter can only convert to value of type " + typeof(Brush).FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }

        public Brush TrueBrush { get; set; }
        public Brush FalseBrush { get; set; }

        public Brush NullBrush { get; set; }
    }
}