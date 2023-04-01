using System;
using System.Windows.Data;

namespace Ideassoccer.BaseStation.UI.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    internal class NullableToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value == null ? "Hidden" : "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
