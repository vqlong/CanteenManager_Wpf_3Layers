using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GUI
{
    public class GetBillTotalPriceConverter : IValueConverter
    {
        public static GetBillTotalPriceConverter Default { get; } = new GetBillTotalPriceConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {           
            if (value is List<BillDetail> list) return list.Sum(bd => bd.TotalPrice);

            return "~~";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
