using System;
using System.Net;
using System.Windows.Data;

namespace Ideassoccer.BaseStation.UI.Converters
{
    class IPEndPointToAddressConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((IPEndPoint)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
