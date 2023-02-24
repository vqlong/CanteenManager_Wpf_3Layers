using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfLibrary.Converters
{
    public class DoubleToStringFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double doubleValue && values[1] is int precision) return Math.Round(doubleValue, precision).ToString();
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && targetTypes[0] == typeof(double))
            {
                if (double.TryParse(stringValue, out double doubleValue))
                {
                    return new object[] { doubleValue };
                }
                return new object[] { DependencyProperty.UnsetValue };
            }
            return new object[] { DependencyProperty.UnsetValue };
        }
    }
}
