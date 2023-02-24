using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        private DialogBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(DialogBox), new PropertyMetadata(string.Empty));
        public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public DialogBoxResult Result { get; private set; } = DialogBoxResult.None;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(e.Source is Button button)
            {
                if (button.Name.Equals(btnOK.Name)) Result = DialogBoxResult.OK;
                if (button.Name.Equals(btnYes.Name)) Result = DialogBoxResult.Yes;
                if (button.Name.Equals(btnNo.Name)) Result = DialogBoxResult.No;
                if (button.Name.Equals(btnCancel.Name)) Result = DialogBoxResult.Cancel;
            }

            Close();
        }
        public static DialogBoxResult Show(string message = "Có biến", string title = "Thông báo", DialogBoxButton button = DialogBoxButton.OK, DialogBoxIcon icon = DialogBoxIcon.Error)
        {
            var dialog = new DialogBox();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.Message = message;
            dialog.Title = title;

            if (button == DialogBoxButton.OK)
            {
                dialog.btnOK.Visibility = Visibility.Visible;
            }
            if (button == DialogBoxButton.OKCancel)
            {
                dialog.btnOK.Visibility = Visibility.Visible;
                dialog.btnCancel.Visibility = Visibility.Visible;
            }
            if (button == DialogBoxButton.YesNo)
            {
                dialog.btnYes.Visibility = Visibility.Visible;
                dialog.btnNo.Visibility = Visibility.Visible;
            }
            if (button == DialogBoxButton.YesNoCancel)
            {
                dialog.btnYes.Visibility = Visibility.Visible;
                dialog.btnNo.Visibility = Visibility.Visible;
                dialog.btnCancel.Visibility = Visibility.Visible;
            }

            if (icon == DialogBoxIcon.Error) dialog.iconError.Visibility = Visibility.Visible;
            if (icon == DialogBoxIcon.Information) dialog.iconInfo.Visibility = Visibility.Visible;
            if (icon == DialogBoxIcon.Warning) dialog.iconWarning.Visibility = Visibility.Visible;
            if (icon == DialogBoxIcon.Question) dialog.iconQuestion.Visibility = Visibility.Visible;

            var result = DialogBoxResult.None;
            dialog.Closed += (s, e) => result = dialog.Result;

            dialog.ShowDialog();

            return result;
        }
    }
}
