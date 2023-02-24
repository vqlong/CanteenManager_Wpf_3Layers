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
    /// Interaction logic for SlideButton.xaml
    /// </summary>
    public partial class SlideButton : ToggleButton, INotifyPropertyChanged
    {
        static SlideButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlideButton), new FrameworkPropertyMetadata(typeof(SlideButton)));
        }
        public SlideButton()
        {
            InitializeComponent();

            _originBackground = Background;
            _originBorderBrush = BorderBrush;

            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(SlideButton), new PropertyMetadata(Brushes.LimeGreen));
        public static readonly DependencyProperty UncheckedBackgroundProperty =
            DependencyProperty.Register("UncheckedBackground", typeof(Brush), typeof(SlideButton), new PropertyMetadata(Brushes.Crimson));
        public static readonly DependencyProperty IsSlidedOnProperty =
            DependencyProperty.Register("IsSlidedOn", typeof(bool), typeof(SlideButton), new PropertyMetadata(false));

        //Brush _thumbBackground = Brushes.Gainsboro;
        //Brush _checkedBackground = Brushes.LimeGreen;
        //Brush _uncheckedBackground = Brushes.Crimson;
        object _checkedContent = "ON";
        object _uncheckedContent = "OFF";
        Brush _mouseEnterBackground = new SolidColorBrush(Color.FromRgb(204, 249, 255));
        Brush _mouseEnterBorderBrush = new SolidColorBrush(Color.FromRgb(0, 172, 223));
        Brush _originBackground;
        Brush _originBorderBrush;

        //public Brush ThumbBackground { get => _thumbBackground; set => OnPropertyChanged(ref _thumbBackground, value); }
        public Brush CheckedBackground { get => (Brush)GetValue(CheckedBackgroundProperty); set => SetValue(CheckedBackgroundProperty, value); }
        public Brush UncheckedBackground { get => (Brush)GetValue(UncheckedBackgroundProperty); set => SetValue(UncheckedBackgroundProperty, value); }
        public bool IsSlidedOn { get => (bool)GetValue(IsSlidedOnProperty); private set => SetValue(IsSlidedOnProperty, value); }
        public object CheckedContent { get => _checkedContent; set => OnPropertyChanged(ref _checkedContent, value); }
        public object UncheckedContent { get => _uncheckedContent; set => OnPropertyChanged(ref _uncheckedContent, value); }
        public Brush MouseEnterBackground { get => _mouseEnterBackground; set => OnPropertyChanged(ref _mouseEnterBackground, value); }
        public Brush MouseEnterBorderBrush { get => _mouseEnterBorderBrush; set => OnPropertyChanged(ref _mouseEnterBorderBrush, value); }

        protected override void OnChecked(RoutedEventArgs e)
        {
            IsSlidedOn = true;

            if (Command is RoutedCommand routedCommand) routedCommand.Execute(CommandParameter, CommandTarget);
            else Command?.Execute(CommandParameter);

            base.OnChecked(e);
        }
        protected override void OnUnchecked(RoutedEventArgs e)
        {
            IsSlidedOn = false;

            if (Command is RoutedCommand routedCommand) routedCommand.Execute(CommandParameter, CommandTarget);
            else Command?.Execute(CommandParameter);

            base.OnUnchecked(e);
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            _originBackground = Background;
            _originBorderBrush = BorderBrush;
            Background = MouseEnterBackground;
            BorderBrush = MouseEnterBorderBrush;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Background = _originBackground;
            BorderBrush = _originBorderBrush;
            base.OnMouseLeave(e);
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var track = (Track)thumb.Tag;

                if (track.Orientation == Orientation.Horizontal)
                {
                    var newValue = track.Value + (e.HorizontalChange * track.Maximum) / track.ActualWidth;
                    if (newValue >= track.Minimum && newValue <= track.Maximum)
                        track.Value = newValue;
                    if (newValue < track.Minimum)
                        track.Value = track.Minimum;
                    if (newValue > track.Maximum)
                        track.Value = track.Maximum;

                }

                if (track.Orientation == Orientation.Vertical)
                {
                    var newValue = track.Value + (e.VerticalChange * -1 * track.Maximum) / track.ActualHeight;
                    if (newValue >= track.Minimum && newValue <= track.Maximum)
                        track.Value = newValue;
                    if (newValue < track.Minimum)
                        track.Value = track.Minimum;
                    if (newValue > track.Maximum)
                        track.Value = track.Maximum;
                }
            }
        }
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RepeatButton button)
            {
                var track = (Track)button.Tag;

                if (track.Value != track.Minimum)
                {
                    track.Value = track.Minimum;
                    return;
                }
                if (track.Value != track.Maximum)
                {
                    track.Value = track.Maximum;
                    return;
                }
            }
        }
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var track = (Track)thumb.Tag;

                if (track.Value <= track.Maximum / 2) track.Value = track.Minimum;
                else track.Value = track.Maximum;
            }
        }
    }
}
