using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfLibrary.UserControls
{
    public class TextInputErrorEventAgrs : RoutedEventArgs
    {
        public TextInputErrorEventAgrs(RoutedEvent routedEvent, object source, string textInput, object errorContent) : base(routedEvent, source)
        {
            TextInput = textInput;
            ErrorContent = errorContent;
        }

        public string TextInput { get; }
        public object ErrorContent { get; }
        public override string ToString() => $"{GetType().FullName}, TextInput: {TextInput}, ErrorContent: {ErrorContent}";
    }
}
