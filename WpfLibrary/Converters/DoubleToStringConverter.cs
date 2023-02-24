using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfLibrary.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        //Dùng để khởi tạo trong x:Static
        public static DoubleToStringConverter Default => new DoubleToStringConverter();

        ////Đánh dấu khi target truyền giá trị cho source để ngăn chặn binding tiếp tục, không cho source truyền lại giá trị vừa cập nhật cho target
        //bool isSourceUpdated;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (isSourceUpdated)
            //{
            //    isSourceUpdated = false;
            //    return Binding.DoNothing;
            //}

            if (value is double doubleValue) return doubleValue.ToString();

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string stringValue)
            {        
                if (double.TryParse(stringValue, out double doubleValue))
                {
                    ////Nếu text đang nhập kết thúc bởi 1 dấu . hoặc 1 số 0
                    //if (Regex.IsMatch(stringValue, @"((^[1-9][\d]*)|(^0))((\.[\d]*0$)|(\.$))")) isSourceUpdated = true;
                    //else isSourceUpdated = false;

                    return doubleValue;
                }
                return DependencyProperty.UnsetValue;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
