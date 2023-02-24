using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GUI
{
    internal class GetRevenueByBillConverter : IValueConverter
    {
        public static GetRevenueByBillConverter Default { get; } = new GetRevenueByBillConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable bills)
            {
                double total = 0;
                foreach (var bill in bills)
                {
                    total += (double)bill.GetType().GetProperties()[5].GetValue(bill);
                }
                return total;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
