using BUS;
using Help;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for wInsertAccount.xaml
    /// </summary>
    public partial class wInsertAccount : Window, IDataErrorInfo
    {
        public wInsertAccount(ResourceDictionary resource)
        {
            Resources.MergedDictionaries.Add(resource);

            InitializeComponent();

            DataContext = this;

            txbUsername.Focus();
        }

        public ObservableCollection<Account> Accounts { get; internal set; }
        public string Username { get; set; }
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == nameof(Username))
                {
                    if (Username is null) return result;

                    var match = Regex.Match(Username, @"^0");
                    if (match.Success) return "Tên đăng nhập không được bắt đầu bằng số 0.";

                    match = Regex.Match(Username, @"^.{4,20}$");
                    if (match.Value.Equals(Username) == false) return "Tên đăng nhập phải bao gồm 4 - 20 ký tự.";

                    match = Regex.Match(Username, @"^[a-zA-Z0-9]*$");
                    if (match.Value.Equals(Username) == false) return "Tên đăng nhập phải là các ký tự 0 - 9, a - z, A - Z.";
                }

                return result;
            }
        }

        public string Error => throw new NotImplementedException();

        private void InsertAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbUsername.Text) || Validation.GetHasError(txbUsername)) e.CanExecute = false;
            else e.CanExecute = true;
        }

        private void InsertAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (DialogBox.Show($"Bạn có thực sự muốn thêm 1 tài khoản mới với tên đăng nhập {txbUsername.Text}?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var account = AccountBUS.Instance.InsertAccount(txbUsername.Text);
                if (account is not null)
                {
                    DialogBox.Show($"Thêm tài khoản mới thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    Accounts.Add(account);
                    Log.Info($"Insert Account - Username: {account.Username}, Displayname: {account.Username}.");
                    Close();
                }
                else
                {
                    DialogBox.Show($"Thêm tài khoản mới thất bại.");
                    Close();
                }

            }
        }
    }
}
