using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AlienJust.Support.Wpf.Converters
{
	public sealed class TextToBrushConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Brush))
			{
				if (value != null)
				{
					var textValue = value.ToString();
					if (textValue == Text1) {
						return Brush1;
					}
					if (textValue == Text2) {
						return Brush2;
					}
					if (textValue == Text3) {
						return Brush3;
					}
					if (textValue == Text4) {
						return Brush4;
					}
					if (textValue == Text5) {
						return Brush5;
					}
				}
				return DefaultBrush;
			}
			throw new InvalidOperationException("Converter can only convert to value of type " + typeof(Brush).FullName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new InvalidOperationException("Converter cannot convert back.");
		}

		public string Text1 { get; set; }
		public string Text2 { get; set; }
		public string Text3 { get; set; }
		public string Text4 { get; set; }
		public string Text5 { get; set; }

		public Brush Brush1 { get; set; }
		public Brush Brush2 { get; set; }
		public Brush Brush3 { get; set; }
		public Brush Brush4 { get; set; }
		public Brush Brush5 { get; set; }

		public Brush DefaultBrush { get; set; }
	}
}