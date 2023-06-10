using System;
using System.Windows.Data;

namespace Ideassoccer.BaseStation.UI.Converters
{
    public class TeamColorToCheckedConverter : IValueConverter
    {
        public object Convert(
            object? value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture
        )
        {
            return value?.ToString() == parameter.ToString();
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
