using BUS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GUI
{
    internal class GetListBillByDateAndPageConverter : IMultiValueConverter
    {
        public static GetListBillByDateAndPageConverter Default { get; } = new GetListBillByDateAndPageConverter();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values[0] as DateTime?) is not null && (values[1] as DateTime?) is not null && values[2] is double pageNumber && values[3] is double pageSize)
            {
                if(parameter is CheckBox checkBox)
                {
                    if (checkBox.IsChecked == true) return BillBUS.Instance.GetListBillByDateAndPage((DateTime)values[0], (DateTime)values[1], (int)pageNumber, (int)pageSize);
                    if (checkBox.IsChecked == false) return BillBUS.Instance.GetListBillByDate((DateTime)values[0], (DateTime)values[1]);
                }
                
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
