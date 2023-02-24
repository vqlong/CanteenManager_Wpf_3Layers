using BUS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GUI
{
    internal class GetMaxPageConverter : IMultiValueConverter
    {
        public static GetMaxPageConverter Default { get; } = new GetMaxPageConverter();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values[0] as DateTime?) is not null && (values[1] as DateTime?) is not null && values[2] is double pageSize)
            {
                var totalRow = BillBUS.Instance.GetNumberBillByDate((DateTime)values[0], (DateTime)values[1]);

                //Trong trường hợp người dùng chọn ngày lung tung, totalRow trả về kết quả 0 => thoát
                if (totalRow <= 0) return DependencyProperty.UnsetValue; ;

                var lastPage = totalRow / (int)pageSize;

                if (totalRow % (int)pageSize != 0) lastPage++;

                return lastPage;

            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
