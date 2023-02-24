using Help;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BUS;
using System.Windows.Threading;
using System.Threading;
using WpfLibrary.UserControls;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace GUI
{
    /// <summary>
    /// Interaction logic for wLogin.xaml
    /// </summary>
    public partial class wLogin : Window
    {
        public wLogin()
        {
            InitializeComponent();

            temp = pwbPassword.Template;
            txbUsername.Focus();
        }

        AccountBUS Account => AccountBUS.Instance;

        //Lưu lại template ban đầu của PasswordBox
        ControlTemplate temp;

        TestConnectionResult testResult;
        void Login()
        {
            frame = new DispatcherFrame();
            btnTestConnection.IsChecked = true;
            Dispatcher.PushFrame(frame);

            if (testResult != TestConnectionResult.Success) return;

            Account loginAccount = Account.Login(txbUsername.Text, pwbPassword.Password);

            if (loginAccount != null)
            {
                Log.Info($"Login Account - Username: {loginAccount.Username}, Displayname: {loginAccount.DisplayName}.");

                txbUsername.Clear();
                pwbPassword.Clear();
                ckbShowPassword.IsChecked = false;
                txbUsername.Focus();
                Hide();

                var tablemng = new wTableManager();
                tablemng.LoginAccount = loginAccount;
                tablemng.Closing += (s, e) => Show();
                tablemng.Show();
            }
            else
            {
                DialogBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Error);
            }
        }
        DispatcherFrame frame;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txbUsername.Text) || string.IsNullOrEmpty(pwbPassword.Password))
            {
                DialogBox.Show("Không được để trống tên đăng nhập hoặc mật khẩu!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Warning);
                return;
            }
            Login();                    
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Khi cần hiện mật khẩu sẽ chuyển sang template này

            var temp = Resources["ShowPasswordBox"] as ControlTemplate;
            pwbPassword.IsTabStop = false;
            pwbPassword.Template = temp;
            if (pwbPassword.ApplyTemplate())
            {
                var txb = temp.FindName("txbShowPassword", pwbPassword) as TextBox;
                txb.Text = pwbPassword.Password;
                txb.TextChanged += (s, e) => pwbPassword.Password = txb.Text;
            }

        }

        //Khi cần ẩn mật khẩu sẽ quay về template ban đầu
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            pwbPassword.Template = temp;
            pwbPassword.IsTabStop = true;
        }

        void TestConnection()
        {
            string connectionString = Config.ConnectionString;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, e) =>
            {
                worker.ReportProgress(0);
                e.Result = Config.DataProvider.TestConnection(e.Argument.ToString());
            };
            worker.ProgressChanged += (s, e) =>
            {
                btnTestConnection.Content = "Đang kết nối...";
                Cursor = Cursors.AppStarting;
                testResult = TestConnectionResult.Waiting;

                borderSpoon.Visibility = Visibility.Visible;
                Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
                StartAnimation();
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                if((bool)e.Result)
                {
                    btnTestConnection.Content = "Kết nối thành công!";
                    testResult = TestConnectionResult.Success;
                }
                else
                {
                    btnTestConnection.Content = "Kết nối thất bại!";
                    testResult = TestConnectionResult.Fail;
                }

                btnTestConnection.IsChecked = false;

                Cursor = Cursors.Arrow;

                if(frame != null) frame.Continue = false;
            };
            worker.RunWorkerAsync(connectionString);
        }

        private void StartAnimation()
        {           
            //Xoay tròn
            var rotateAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(2)), FillBehavior.Stop);
            rotateAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(6));
            var clock = rotateAnimation.CreateClock();
            imgSpoon.RenderTransform = new RotateTransform();
            imgSpoon.RenderTransform.ApplyAnimationClock(RotateTransform.AngleProperty, clock);
            imgNoodle.RenderTransform = new RotateTransform();
            imgNoodle.RenderTransform.ApplyAnimationClock(RotateTransform.AngleProperty, clock);

            //Tịnh tiến
            var storyBoard = new Storyboard();
            storyBoard.Completed += (s, e) => borderSpoon.Visibility = Visibility.Collapsed;
            var spoonTraverseAnimation = new DoubleAnimation(0, borderSpoon.ActualWidth - 160, new Duration(TimeSpan.FromSeconds(6)), FillBehavior.Stop);
            storyBoard.Children.Add(spoonTraverseAnimation);
            Storyboard.SetTargetName(spoonTraverseAnimation, imgSpoon.Name);
            Storyboard.SetTargetProperty(spoonTraverseAnimation, new PropertyPath("(Canvas.Left)"));

            var noodleTraverseAnimation = new DoubleAnimation(80, borderSpoon.ActualWidth - 80, new Duration(TimeSpan.FromSeconds(6)), FillBehavior.Stop);
            storyBoard.Children.Add(noodleTraverseAnimation);
            Storyboard.SetTargetName(noodleTraverseAnimation, imgNoodle.Name);
            Storyboard.SetTargetProperty(noodleTraverseAnimation, new PropertyPath("(Canvas.Left)"));

            storyBoard.Begin(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(DialogBox.Show("Bạn có thực sự muốn thoát?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.No) e.Cancel = true;
        }

        private void btnTestConnection_Checked(object sender, RoutedEventArgs e) => TestConnection();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"From: {e.OldFocus}\n ~To: {e.NewFocus}");
        }
    }
}
