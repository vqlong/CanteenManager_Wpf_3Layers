using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfLibrary
{
    public class AttachedManager
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(AttachedManager),
                new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty DoubleProperty =
            DependencyProperty.RegisterAttached(
                "Double",
                typeof(double),
                typeof(AttachedManager),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty TagProperty =
            DependencyProperty.RegisterAttached(
                "Tag",
                typeof(object),
                typeof(AttachedManager),
                new PropertyMetadata(null));
        public static CornerRadius GetCornerRadius(DependencyObject target)
        {
            return (CornerRadius)target.GetValue(CornerRadiusProperty);
        }
        public static void SetCornerRadius(DependencyObject target, CornerRadius value)
        {
            target.SetValue(CornerRadiusProperty, value);
        }
        public static double GetDouble(DependencyObject target)
        {
            return (double)target.GetValue(DoubleProperty);
        }
        public static void SetDouble(DependencyObject target, double value)
        {
            target.SetValue(DoubleProperty, value);
        }
        public static object GetTag(DependencyObject target)
        {
            return target.GetValue(TagProperty);
        }
        public static void SetTag(DependencyObject target, object value)
        {
            target.SetValue(TagProperty, value);
        }

    }
}
