using BUS;
using Help;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfLibrary.UserControls;

namespace GUI
{
    /// <summary>
    /// Interaction logic for wAccountProfile.xaml
    /// </summary>
    public partial class wAccountProfile : Window, IDataErrorInfo
    {
        public wAccountProfile(ResourceDictionary resource)
        {
            Resources.MergedDictionaries.Add(resource);

            InitializeComponent(); 

            DataContext = this;

        }
        public static RoutedCommand UpdateAccount { get; } = new RoutedCommand("UpdateAccount", typeof(wAccountProfile));
        public Account LoginAccount { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == nameof(NewPassword))
                {
                    if(NewPassword is null) return result;

                    if(NewPassword.Equals(ConfirmPassword))
                    {
                        //Trường hợp txbConfirmPassword đang error mà txbNewPassword được nhập lại cho khớp với txbConfirmPassword
                        //Gán đi gán lại để kích hoạt binding => validate lại txbConfirmPassword
                        txbConfirmPassword.Text = "@";
                        txbConfirmPassword.Text = NewPassword;
                    }
                    else
                    {
                        if(ConfirmPassword != null)
                        {
                            var temp = ConfirmPassword;
                            txbConfirmPassword.Text = "@";
                            txbConfirmPassword.Text = temp;
                        }
                    }

                    var match = Regex.Match(NewPassword, @"^0");
                    if (match.Success) return "Mật khẩu không được bắt đầu bằng số 0.";

                    match = Regex.Match(NewPassword, @"^.{6,20}$");
                    if (match.Value.Equals(NewPassword) == false) return "Mật khẩu phải bao gồm 6 - 20 ký tự.";

                    match = Regex.Match(NewPassword, @"^[a-zA-Z0-9]*$");
                    if (match.Value.Equals(NewPassword) == false) return "Mật khẩu phải là các ký tự 0 - 9, a - z, A - Z.";                  
                }

                if(columnName == nameof(ConfirmPassword))
                {
                    if (ConfirmPassword != NewPassword) return "Mật khẩu nhập lại không trùng khớp.";
                }

                return result;
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && passwordBox.TemplatedParent is TextBox textBox) textBox.Text = passwordBox.Password;
        }

        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && passwordBox.TemplatedParent is TextBox textBox) passwordBox.Password = textBox.Text;
        }

        private void UpdateAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbPassword.Text) || Validation.GetHasError(txbNewPassword) || Validation.GetHasError(txbConfirmPassword)) e.CanExecute = false;
            else e.CanExecute = true;
        }

        private void UpdateAccount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var loginAccount = AccountBUS.Instance.Login(LoginAccount.Username, Password);
            if (loginAccount != null)
            {
                var result = AccountBUS.Instance.Update(LoginAccount.Username, LoginAccount.DisplayName, NewPassword);
                if (result.Item1 || result.Item2)
                {
                    Log.Info($"Update Account - Username: {LoginAccount.Username}, Displayname: {LoginAccount.DisplayName}.");

                    string info = "";
                    if (result.Item1) info = "tên hiển thị";
                    if (result.Item1 && result.Item2) info += " và mật khẩu";
                    DialogBox.Show($"Cập nhật {info} thành công!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    txbPassword.Clear();
                    txbNewPassword.Clear();
                    txbConfirmPassword.Clear();
                    //Đổi qua lại 2 template để reset passwordbox
                    ckbShowPassword.IsChecked = true;
                    ckbShowPassword.IsChecked = false;
                }
                else
                {
                    DialogBox.Show("Cập nhật thất bại!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Error);
                }
            }
            else
            {
                DialogBox.Show("Sai mật khẩu!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Error);

                txbPassword.Clear();
                txbNewPassword.Clear();
                txbConfirmPassword.Clear();
                ckbShowPassword.IsChecked = true;
                ckbShowPassword.IsChecked = false;
            }
        }
    }
}
