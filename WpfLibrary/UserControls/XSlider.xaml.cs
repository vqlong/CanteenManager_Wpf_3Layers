using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for XSlider.xaml
    /// </summary>
    public partial class XSlider : Slider, INotifyPropertyChanged
    {
        static XSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XSlider), new FrameworkPropertyMetadata(typeof(XSlider)));
        }
        public XSlider()
        {
            InitializeComponent();

            DataContext = this;

            Template = (ControlTemplate)TryFindResource("HorizontalSlider");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            //if (value is Visibility visibility && propertyName == "ShowThumb")
            //{
            //    if (visibility == Visibility.Collapsed) value = (T)Convert.ChangeType(Visibility.Hidden, typeof(T));
            //}
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static readonly DependencyProperty IsSeekBarProperty =
            DependencyProperty.Register("IsSeekBar", typeof(bool), typeof(XSlider), new PropertyMetadata(false));

        public static readonly DependencyProperty ShowThumbProperty =
            DependencyProperty.Register("ShowThumb", typeof(Visibility), typeof(XSlider), new PropertyMetadata(Visibility.Visible, ShowThumbChangedCallback, CoerceShowThumbCallback));

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(XSliderMode), typeof(XSlider), new PropertyMetadata(XSliderMode.HorizontalSlider, ModeChangedCallback));

        Visibility _showValue = Visibility.Visible;
        Visibility _showMaxMin = Visibility.Visible;
        Brush _valueForeground = Brushes.Gray;
        Brush _thumbBackground = new SolidColorBrush(Color.FromRgb(255, 216, 111));
        Brush _decreaseButtonBackground = Brushes.YellowGreen;
        Brush _increaseButtonBackground = Brushes.Red;
        Brush _tickFill = Brushes.LightGray;
        double _tickHeight = 5;
        double _tickWidth = 5;
        double _trackHeight = 5;
        double _trackWidth = 5;
        string _unit = "";

        public Visibility ShowValue { get => _showValue; set => OnPropertyChanged(ref _showValue, value); }
        public Visibility ShowMaxMin { get => _showMaxMin; set => OnPropertyChanged(ref _showMaxMin, value); }
        public Visibility ShowThumb { get => (Visibility)GetValue(ShowThumbProperty); set => SetValue(ShowThumbProperty, value); }
        public XSliderMode Mode { get => (XSliderMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
        public Brush ValueForeground { get => _valueForeground; set => OnPropertyChanged(ref _valueForeground, value); }
        public Brush ThumbBackground { get => _thumbBackground; set => OnPropertyChanged(ref _thumbBackground, value); }
        public Brush DecreaseButtonBackground { get => _decreaseButtonBackground; set => OnPropertyChanged(ref _decreaseButtonBackground, value); }
        public Brush IncreaseButtonBackground { get => _increaseButtonBackground; set => OnPropertyChanged(ref _increaseButtonBackground, value); }
        public Brush TickFill { get => _tickFill; set => OnPropertyChanged(ref _tickFill, value); }
        public double TickHeight { get => _tickHeight; set => OnPropertyChanged(ref _tickHeight, value); }
        public double TickWidth { get => _tickWidth; set => OnPropertyChanged(ref _tickWidth, value); }
        public double TrackHeight { get => _trackHeight; set => OnPropertyChanged(ref _trackHeight, value); }
        public double TrackWidth { get => _trackWidth; set => OnPropertyChanged(ref _trackWidth, value); }
        public bool IsSeekBar { get => (bool)GetValue(IsSeekBarProperty); internal set => SetValue(IsSeekBarProperty, value); }
        public bool IsDragging { get; private set; }
        public string Unit { get => _unit; set => OnPropertyChanged(ref _unit, value); }
        static void ModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (XSlider)d;
            var mode = (XSliderMode)e.NewValue;

            if (mode == XSliderMode.HorizontalSeekSlider)
            {
                slider.Orientation = Orientation.Horizontal;
                slider.IsSeekBar = true;
                slider.Template = (ControlTemplate)slider.TryFindResource("HorizontalSlider");
            }
            if (mode == XSliderMode.VerticalSeekSlider)
            {
                slider.Orientation = Orientation.Vertical;
                slider.IsSeekBar = true;
                slider.Template = (ControlTemplate)slider.TryFindResource("VerticalSlider");
            }
            if (mode == XSliderMode.HorizontalSlider)
            {
                slider.Orientation = Orientation.Horizontal;
                slider.IsSeekBar = false;
                slider.Template = (ControlTemplate)slider.TryFindResource("HorizontalSlider");
            }
            if (mode == XSliderMode.VerticalSlider)
            {
                slider.Orientation = Orientation.Vertical;
                slider.IsSeekBar = false;
                slider.Template = (ControlTemplate)slider.TryFindResource("VerticalSlider");
            }
        }

        static void ShowThumbChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        static object CoerceShowThumbCallback(DependencyObject d, object baseValue)
        {
            if (baseValue is Visibility visibility)
                if (visibility == Visibility.Collapsed)
                    return Visibility.Hidden;

            return baseValue;
        }
        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            if (e.OriginalSource is Thumb thumb) IsDragging = thumb.IsDragging;
            OnPropertyChanged("IsDragging");
            base.OnThumbDragDelta(e);
        }
        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (e.OriginalSource is Thumb thumb) IsDragging = thumb.IsDragging;
            OnPropertyChanged("IsDragging");
            base.OnThumbDragCompleted(e);
        }
    }
}
