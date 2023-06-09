using System;
using System.Windows;
using System.Windows.Data;

namespace Ideassoccer.BaseStation.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    internal class BooleanToVisibilityCollapseConverter : IValueConverter
    {
        public object Convert(
            object? value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture
        )
        {
            if (value == null || (bool)value == false)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture
        )
        {
            throw new NotSupportedException();
        }
    }
}
