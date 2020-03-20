using System;
using System.Globalization;
using Xamarin.Forms;

namespace Saplin.CPDT.UICore.ValueConverters
{
    public class NumberToDotsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int) return "".PadRight((int)value,'.');

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}