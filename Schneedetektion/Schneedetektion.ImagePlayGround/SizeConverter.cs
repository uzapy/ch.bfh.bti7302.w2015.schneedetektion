using System;
using System.Globalization;
using System.Windows.Data;

namespace Schneedetektion.ImagePlayGround
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 5;
        }
    }
}
