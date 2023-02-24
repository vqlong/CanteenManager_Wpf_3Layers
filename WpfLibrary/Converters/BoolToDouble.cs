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
    public class BoolToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (boolValue == true) return 1;
                if (boolValue == false) return 0;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double doubleValue)
            {
                if (doubleValue == 1) return true;
                if (doubleValue == 0) return false;
                return Binding.DoNothing;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
