using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfLibrary.UserControls
{
    public class ValueChangedEventAgrs : RoutedEventArgs
    {
        public ValueChangedEventAgrs(RoutedEvent routedEvent, object source, object oldValue, object newValue) : base(routedEvent, source)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        public object NewValue { get; }
        public object OldValue { get; }
        public override string ToString() => $"{GetType().FullName}, OldValue: {OldValue}, NewValue: {NewValue}";
    }
}
