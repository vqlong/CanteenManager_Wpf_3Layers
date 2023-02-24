using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl, INotifyPropertyChanged
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }
        public NumericUpDown()
        {
            InitializeComponent();

            Loaded += (s, e) => 
            {
                var txbInput = (TextBox)Template.FindName("txbInput", this);
                //Nếu không set CornerRadius trong XAML thì nó sẽ bằng 1/2 ActualHeight (tròn xoe)
                //đồng thời error template cũng thay đổi
                if (CornerRadius == new CornerRadius(double.NaN))
                {
                    CornerRadius = new CornerRadius(ActualHeight / 2);
                    Validation.SetErrorTemplate(txbInput, (ControlTemplate)TryFindResource("ErrorTemplate3"));
                }
            };

            CommandBindings.Add(new CommandBinding(ValueUp, (s, e) => SetCurrentValue(ValueProperty, Value + Interval), (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ValueDown, (s, e) => SetCurrentValue(ValueProperty, Value - Interval), (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ValueMax, (s, e) => SetCurrentValue(ValueProperty, MaxValue), (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ValueMin, (s, e) => SetCurrentValue(ValueProperty, MinValue), (s, e) => e.CanExecute = true));
        }

        #region DependencyProperty & RoutedEvent
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(0.0, ValueChangedCallback, CoerceValueCallback),
                ValidateValueCallback);

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                "MaxValue",
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                "MinValue",
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(double.NegativeInfinity));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ValueChanged",
                RoutingStrategy.Bubble,
                typeof(ValueChangedEventHandler),
                typeof(NumericUpDown));

        public static readonly RoutedEvent TextInputErrorEvent =
            EventManager.RegisterRoutedEvent(
                "TextInputError",
                RoutingStrategy.Bubble,
                typeof(TextInputErrorEventHandler),
                typeof(NumericUpDown));

        public static readonly DependencyProperty NumberAlignmentProperty =
            DependencyProperty.Register(
                "NumberAlignment",
                typeof(TextAlignment),
                typeof(NumericUpDown),
                new PropertyMetadata(TextAlignment.Center));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(NumericUpDown),
                new PropertyMetadata(new CornerRadius(double.NaN)));
        #endregion

        public static RoutedCommand ValueUp { get; } = new RoutedCommand("ValueUp", typeof(NumericUpDown));
        public static RoutedCommand ValueDown { get; } = new RoutedCommand("ValueDown", typeof(NumericUpDown));
        public static RoutedCommand ValueMax { get; } = new RoutedCommand("ValueMax", typeof(NumericUpDown));
        public static RoutedCommand ValueMin { get; } = new RoutedCommand("ValueMin", typeof(NumericUpDown));

        #region Property       
        double _value2;
        double _interval = 1;
        int _precision = 6;
        CornerRadius _cornerRadius = new CornerRadius(double.NaN);
        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public double Value2 { get => _value2; set => OnPropertyChanged(ref _value2, value); }
        public double Interval { get => _interval; set => OnPropertyChanged(ref _interval, value); }     
        public double MaxValue { get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public double MinValue { get => (double)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public int Precision { get => _precision; set => OnPropertyChanged(ref _precision, value); }
        /// <summary>
        /// Bo tròn 4 góc cho NumericUpDown, mặc định bằng 1/2 chiều cao (ActualHeight).
        /// </summary>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
        /// <summary>
        /// Bo tròn 4 góc cho 2 Button tăng/giảm giá trị.
        /// </summary>
        public CornerRadius ButtonCornerRadius 
        { 
            get => AttachedManager.GetCornerRadius(this);
            set
            {
                AttachedManager.SetCornerRadius(this, value);
                OnPropertyChanged();
            }
        }
        public TextAlignment NumberAlignment { get => (TextAlignment)GetValue(NumberAlignmentProperty); set => SetValue(NumberAlignmentProperty, value); }
        #endregion

        #region Event
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Xảy ra khi Value được gán giá trị mới, khác giá trị hiện tại.
        /// </summary>
        public event ValueChangedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        /// <summary>
        /// Xảy ra khi không thể convert chuỗi nhập vào thành Value.
        /// </summary>
        public event TextInputErrorEventHandler TextInputError
        {
            add { AddHandler(TextInputErrorEvent, value); }
            remove { RemoveHandler(TextInputErrorEvent, value); }
        }
        #endregion

        #region Method
        public override string ToString() => $"{GetType().FullName} Value: {Value}";
        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericUpDown)d).RaiseEvent(new ValueChangedEventAgrs(ValueChangedEvent, d, e.OldValue, e.NewValue));
        }
        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue is double doubleValue)
            {
                var nmrud = (NumericUpDown)d;
                if (nmrud.MaxValue != double.NaN && nmrud.MaxValue < doubleValue) return nmrud.MaxValue;
                if (nmrud.MinValue != double.NaN && nmrud.MinValue > doubleValue) return nmrud.MinValue;
                return doubleValue;
            }
            return double.NaN;
        }
        private static bool ValidateValueCallback(object value)
        {
            return value is double;
        }
        private void OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox chỉ cho phép nhập số

            var textBox = sender as TextBox;

            if (string.IsNullOrEmpty(textBox.Text)) return;

            var textInputIsDouble = double.TryParse(textBox.Text, out double value);
            if (textInputIsDouble)
            {
                if (Value2 != value) Value2 = value;
            }
            else
            {
                if (e.Changes.Any())
                {
                    var textChanges = e.Changes.Where(tc => tc.AddedLength > 0);

                    var temp = new StringBuilder(textBox.Text);
                    foreach (var change in textChanges)
                    {
                        //tạo 1 chuỗi ? với độ dài tương đương chuỗi mới nhập 
                        string s = new string('?', change.AddedLength);
                        //thay thế chuỗi mới nhập bằng chuỗi ?
                        temp = temp.Replace(textBox.Text.Substring(change.Offset, change.AddedLength), s, change.Offset, change.AddedLength);
                    }
                    //xoá toàn bộ ?
                    temp.Replace("?", "");
                    //set trực tiếp giá trị cho Text sẽ làm binding tịt vì nó được ưu tiên cao hơn 
                    //textBox.Text = temp.ToString();
                    //set kiểu này mới chạy được
                    //You can use SetCurrentValue whenever you want to set a property value without giving that value the precedence level of a local value.
                    //Similarly, you can use SetCurrentValue to change the value of a property without overwriting a binding.
                    textBox.SetCurrentValue(TextBox.TextProperty, temp.ToString());
                    textBox.Select(temp.Length, 0);

                }
            }
        }
        private void IncreaseButton_Click(object sender, RoutedEventArgs e) => SetCurrentValue(ValueProperty, Value + Interval);
        private void DecreaseButton_Click(object sender, RoutedEventArgs e) => SetCurrentValue(ValueProperty, Value - Interval);
        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            if(e.Action == ValidationErrorEventAction.Added)
            {
                if (e.Source is TextBox textBox)
                {
                    //if(textBox.Parent is Border border) border.Background = Brushes.LightPink;
                    RaiseEvent(new TextInputErrorEventAgrs(TextInputErrorEvent, this, textBox.Text, e.Error.ErrorContent));
                }
                
            }
                
        } 
        #endregion
    }
}
