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
    //Dựa vào Value, Minimum, Maximum, Width/Height của slider để tính Canvas.Left/Bottom của textbox hiển thị value
    //parameter là chiều rộng của Thumb
    public class DoubleToPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double sliderValue && values[1] is double minimum && values[2] is double maximum && values[3] is double sliderSize)
            {
                var ratio = (sliderValue - minimum) / (maximum - minimum);

                return (sliderSize - (int.TryParse(parameter.ToString(), out int result) ? result : 0)) * ratio;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
