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
    public class PathToIndexConverter : IValueConverter
    {
        public static PathToIndexConverter Default { get; } = new PathToIndexConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is CollectionViewSource viewSource)
            {
                return $"{((CollectionView)viewSource.View).IndexOf(stringValue) + 1}.";
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
