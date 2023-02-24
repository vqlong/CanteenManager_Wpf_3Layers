using BUS;
using Help;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Reporting.WinForms;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using WpfLibrary.UserControls;
using Table = Models.Table;

namespace GUI
{
    /// <summary>
    /// Interaction logic for wAdmin.xaml
    /// </summary>
    public partial class wAdmin : Window
    {
        public wAdmin(ResourceDictionary resource)
        {
            Resources.MergedDictionaries.Add(resource);

            InitializeComponent();

            DataContext = this;
        }
        public static RoutedCommand UpdateFood { get; } = new RoutedCommand("UpdateFood", typeof(wAdmin));
        public static RoutedCommand DeleteFood { get; } = new RoutedCommand("DeleteFood", typeof(wAdmin));
        public static RoutedCommand InsertFood { get; } = new RoutedCommand("InsertFood", typeof(wAdmin));
        public static RoutedCommand UpdateCategory { get; } = new RoutedCommand("UpdateCategory", typeof(wAdmin));
        public static RoutedCommand DeleteCategory { get; } = new RoutedCommand("DeleteCategory", typeof(wAdmin));
        public static RoutedCommand InsertCategory { get; } = new RoutedCommand("InsertCategory", typeof(wAdmin));
        public static RoutedCommand UpdateTable { get; } = new RoutedCommand("UpdateTable", typeof(wAdmin));
        public static RoutedCommand DeleteTable { get; } = new RoutedCommand("DeleteTable", typeof(wAdmin));
        public static RoutedCommand InsertTable { get; } = new RoutedCommand("InsertTable", typeof(wAdmin));
        public static RoutedCommand UpdateAccount { get; } = new RoutedCommand("UpdateAccount", typeof(wAdmin));
        public static RoutedCommand DeleteAccount { get; } = new RoutedCommand("DeleteAccount", typeof(wAdmin));
        public static RoutedCommand InsertAccount { get; } = new RoutedCommand("InsertAccount", typeof(wAdmin));
        public static RoutedCommand ResetPassword { get; } = new RoutedCommand("ResetPassword", typeof(wAdmin));
        public Account LoginAccount { get; set; }
        public ObservableCollection<Category> Categories { get; internal set; }// = new ObservableCollection<Category>(CategoryBUS.Instance.GetListCategory());
        public ObservableCollection<Table> Tables { get; internal set; } //= new ObservableCollection<Table>(TableBUS.Instance.GetListTable());
        public ObservableCollection<Food> Foods { get; } = new ObservableCollection<Food>(FoodBUS.Instance.GetListFood());
        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>(AccountBUS.Instance.GetListAccount());
        public static DateTime FirstDayInMonth { get; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public static DateTime LastDayInMonth { get; } = FirstDayInMonth.AddMonths(1).AddSeconds(-1);
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                if (tabControl.Items.Count <= 0) return;
                var selectedTab = e.AddedItems.Count > 0 ? (TabItem)e.AddedItems[0] : null;
                var unselectedTab = e.RemovedItems.Count > 0 ? (TabItem)e.RemovedItems[0] : null;
                if (selectedTab != null && selectedTab.Equals(tabControl.Items[0])) selectedTab.Margin = new Thickness(2, 0, -2, -15);
                if (unselectedTab != null && unselectedTab.Equals(tabControl.Items[0])) unselectedTab.Margin = new Thickness(10, 0, 0, 0);
            }
 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(lsvFoods.ItemsSource);
            view.Filter = (obj) => (obj as Food).Name.IndexOf(txbSearchFood.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;

            var view2 = CollectionViewSource.GetDefaultView(lsvCategories.ItemsSource);
            view2.Filter = (obj) => (obj as Category).Name.IndexOf(txbSearchCategory.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = e.Source as TextBox;
            if (textbox.Name.Equals(txbSearchFood.Name))
            {
                var view = CollectionViewSource.GetDefaultView(lsvFoods.ItemsSource);
                view.Refresh();
            }
            if (textbox.Name.Equals(txbSearchCategory.Name))
            {
                var view = CollectionViewSource.GetDefaultView(lsvCategories.ItemsSource);
                view.Refresh();
            }
        }

        private void UpdateFood_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvFoods != null && lsvFoods.SelectedItem != null) e.CanExecute = true;
        }

        private void UpdateFood_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var food = lsvFoods.SelectedItem as Food;
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật món ăn này?\n" +
                $"Id: {food.Id}\n" +
                $"Tên: {txbFoodName.Text}\n" +
                $"Mục: {cbFoodCategories.Text}\n" +
                $"Giá: {nmPrice.Value.ToString("C1", CultureInfo.GetCultureInfo("vi-vn"))}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (FoodBUS.Instance.UpdateFood(food.Id, txbFoodName.Text, (int)cbFoodCategories.SelectedValue, nmPrice.Value, (UsingState)cbFoodStatus.SelectedItem))
                {
                    DialogBox.Show($"Cập nhật món ăn {txbFoodName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadFood(food.Id);
                    ReloadCategory((int)cbFoodCategories.SelectedValue);
                    if (food.CategoryId != (int)cbFoodCategories.SelectedValue) ReloadCategory(food.CategoryId);
                    Log.Info($"Update Food - Id: {food.Id}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật món ăn {txbFoodName.Text} thất bại.");
                }
                    
            }
        }

        private void ReloadFood(int foodId)
        {
            var oldFood = Foods.FirstOrDefault(c => c.Id == foodId);
            if (oldFood == null) return;
            var index = Foods.IndexOf(oldFood);
            Foods.RemoveAt(index);
            var newFood = FoodBUS.Instance.GetFoodById(foodId);
            Foods.Insert(index, newFood);
            lsvFoods.SelectedIndex = index;
        }

        private void ReloadCategory(int categoryId)
        {
            var oldCategory = Categories.FirstOrDefault(c => c.Id == categoryId);
            if (oldCategory == null) return;
            var index = Categories.IndexOf(oldCategory);
            Categories.RemoveAt(index);
            var newCategory = CategoryBUS.Instance.GetCategoryById(categoryId);
            Categories.Insert(index, newCategory);
            lsvCategories.SelectedIndex = index;
        }
        private void DeleteFood_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var food = lsvFoods.SelectedItem as Food;
            if (DialogBox.Show($"Bạn có thực sự muốn xoá món ăn {food.Name}?\nMón ăn bị xoá sẽ chuyển sang trạng thái [Dừng bán].",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (FoodBUS.Instance.DeleteFood(food.Id))
                {
                    DialogBox.Show($"Xoá món ăn {food.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadFood(food.Id);
                    ReloadCategory(food.CategoryId);
                    Log.Info($"Delete Food - Id: {food.Id}.");
                }
                else
                {
                    DialogBox.Show($"Xoá món ăn {food.Name} thất bại.");
                }
                    
            }
        }

        private void InsertFood_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txbFoodName.Text != "" && cbFoodCategories.SelectedItem != null && nmPrice.Value != 0 && cbFoodStatus.SelectedItem != null && (UsingState)cbFoodStatus.SelectedItem == UsingState.Serving) e.CanExecute = true;
        }

        private void InsertFood_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm món ăn này?\n" +
                $"Tên: {txbFoodName.Text}\n" +
                $"Mục: {cbFoodCategories.Text}\n" +
                $"Giá: {nmPrice.Value.ToString("C1", CultureInfo.GetCultureInfo("vi-vn"))}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var food = FoodBUS.Instance.InsertFood(txbFoodName.Text, (int)cbFoodCategories.SelectedValue, nmPrice.Value);
                if (food != null)
                {
                    DialogBox.Show($"Thêm món ăn {txbFoodName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    Foods.Add(food);
                    lsvFoods.SelectedItem = food;
                    ReloadCategory(food.CategoryId);
                    Log.Info($"Insert Food - Id: {food.Id}.");
                }
                else
                {
                    DialogBox.Show($"Thêm món ăn {txbFoodName.Text} thất bại.");
                }
                    
            }
        }

        private void UpdateCategory_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvCategories != null && lsvCategories.SelectedItem != null) e.CanExecute = true;
        }

        private void UpdateCategory_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var category = lsvCategories.SelectedItem as Category;
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật danh mục này?\n" +
                $"Id: {category.Id}\n" +
                $"Tên: {txbCategoryName.Text}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (CategoryBUS.Instance.UpdateCategory(category.Id, txbCategoryName.Text, (UsingState)cbCategoryStatus.SelectedItem))
                {
                    DialogBox.Show($"Cập nhật danh mục {txbCategoryName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadCategory(category.Id);
                    if(category.CategoryStatus != (UsingState)cbCategoryStatus.SelectedItem)
                    {
                        foreach (var food in Foods)
                        {
                            if(food.CategoryId == category.Id) food.FoodStatus = (UsingState)cbCategoryStatus.SelectedItem;
                        }
                        var view = CollectionViewSource.GetDefaultView(lsvFoods.ItemsSource);
                        view.Refresh();
                    }
                    Log.Info($"Update Category - Id: {category.Id}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật danh mục {txbCategoryName.Text} thất bại.");
                }
                    
            }
        }

        private void InsertCategory_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txbCategoryName.Text != "" && cbCategoryStatus.SelectedItem != null && (UsingState)cbCategoryStatus.SelectedItem == UsingState.Serving) e.CanExecute = true;
        }

        private void InsertCategory_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm danh mục này?\n" +
                $"Tên: {txbCategoryName.Text}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var category = CategoryBUS.Instance.InsertCategory(txbCategoryName.Text);
                if (category is not null)
                {
                    DialogBox.Show($"Thêm danh mục {txbCategoryName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    Categories.Add(category);
                    lsvCategories.SelectedItem = category;
                    Log.Info($"Insert Category - Id: {category.Id}.");
                }
                else
                {
                    DialogBox.Show($"Thêm danh mục {txbCategoryName.Text} thất bại.");
                }
                    
            }
        }

        private void DeleteCategory_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var category = lsvCategories.SelectedItem as Category;
            if (DialogBox.Show($"Bạn có thực sự muốn xoá danh mục {category.Name}?\ndanh mục bị xoá sẽ chuyển sang trạng thái [Dừng bán].",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (CategoryBUS.Instance.DeleteCategory(category.Id))
                {
                    DialogBox.Show($"Xoá danh mục {category.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadCategory(category.Id);
                    foreach (var food in Foods)
                    {
                        if (food.CategoryId == category.Id) food.FoodStatus = UsingState.StopServing;
                    }
                    var view = CollectionViewSource.GetDefaultView(lsvFoods.ItemsSource);
                    view.Refresh();
                    Log.Info($"Delete Category - Id: {category.Id}.");
                }
                else
                {
                    DialogBox.Show($"Xoá danh mục {category.Name} thất bại.");
                }
                    
            }
        }

        private void UpdateTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvTables != null && lsvTables.SelectedItem != null)
                if (lsvTables.SelectedItem is Table table)
                    if (table.Status.Equals("Trống")) e.CanExecute = true;
        }

        private void UpdateTable_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var table = lsvTables.SelectedItem as Table;
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật bàn ăn này?\n" +
                $"Id: {table.Id}\n" +
                $"Tên: {txbTableName.Text}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (TableBUS.Instance.UpdateTable(table.Id, txbTableName.Text, (UsingState)cbTableStatus.SelectedItem))
                {
                    DialogBox.Show($"Cập nhật {txbTableName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadTable(table.Id);

                    Log.Info($"Update Table - Id: {table.Id}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật {txbTableName.Text} thất bại.");
                }

            }
        }

        private void ReloadTable(int tableId)
        {
            var oldTable = Tables.FirstOrDefault(t => t.Id == tableId);
            if (oldTable == null) return;
            var index = Tables.IndexOf(oldTable);
            Tables.RemoveAt(index);
            var newTable = TableBUS.Instance.GetTableById(tableId);
            Tables.Insert(index, newTable);
            lsvTables.SelectedIndex = index;
        }

        private void InsertTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txbTableName.Text != "" && cbTableStatus.SelectedItem != null && (UsingState)cbTableStatus.SelectedItem == UsingState.Serving) e.CanExecute = true;
        }

        private void InsertTable_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm 1 bàn ăn mới?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var table = TableBUS.Instance.InsertTable();
                if (table is not null)
                {
                    DialogBox.Show($"Thêm bàn ăn mới thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    Tables.Add(table);
                    lsvTables.SelectedItem = table;
                    Log.Info($"Insert Table - Id: {table.Id}.");
                }
                else
                {
                    DialogBox.Show($"Thêm bàn ăn mới thất bại.");
                }

            }
        }

        private void DeleteTable_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var table = lsvTables.SelectedItem as Table;
            if (DialogBox.Show($"Bạn có thực sự muốn xoá {table.Name}?\nbàn ăn bị xoá sẽ chuyển sang trạng thái [Dừng bán].",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (TableBUS.Instance.DeleteTable(table.Id))
                {
                    DialogBox.Show($"Xoá {table.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadTable(table.Id);

                    Log.Info($"Delete Table - Id: {table.Id}.");
                }
                else
                {
                    DialogBox.Show($"Xoá {table.Name} thất bại.");
                }

            }
        }

        private void UpdateAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvAccounts != null && lsvAccounts.SelectedItem != null)
            {
                if (lsvAccounts.SelectedItem is Account account)
                {
                    if (account.Type == AccountType.Staff || (account.Type == AccountType.Admin && account.Username == LoginAccount.Username)) e.CanExecute = true;
                }
                    
            }

        }

        private void UpdateAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var account = lsvAccounts.SelectedItem as Account;
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật tài khoản này?\n" +
                $"Tên đăng nhập: {account.Username}\n" +
                $"Tên hiển thị: {txbDisplayName.Text}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (AccountBUS.Instance.Update(account.Username, txbDisplayName.Text).Item1)
                {
                    DialogBox.Show($"Cập nhật tài khoản {txbDisplayName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadAccounts();

                    Log.Info($"Update Account - Username: {account.Username}, Displayname: {account.Username}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật tài khoản {txbDisplayName.Text} thất bại.");
                }

            }
        }

        private void ReloadAccounts()
        {
            Accounts.Clear();
            var accounts = AccountBUS.Instance.GetListAccount();
            accounts.ForEach(a => Accounts.Add(a));
        }

        private void DeleteAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var account = lsvAccounts.SelectedItem as Account;
            if (DialogBox.Show($"Bạn có thực sự muốn xoá tài khoản {account.Username}?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (AccountBUS.Instance.DeleteAccount(account.Username))
                {
                    DialogBox.Show($"Xoá tài khoản {account.Username} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    ReloadAccounts();

                    Log.Info($"Delete Account - Username: {account.Username}, Displayname: {account.Username}.");
                }
                else
                {
                    DialogBox.Show($"Xoá tài khoản {account.Username} thất bại.");
                }

            }
        }

        private void ResetPassword_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var account = lsvAccounts.SelectedItem as Account;
            if (DialogBox.Show("Bạn có thực sự muốn đặt lại mật khẩu cho tài khoản này?\n" +
                $"Tên đăng nhập: {account.Username}\n" +
                $"Tên hiển thị: {txbDisplayName.Text}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (AccountBUS.Instance.Update(account.Username, null, "123456").Item2)
                {
                    DialogBox.Show($"Đặt lại mật khẩu cho tài khoản {txbDisplayName.Text} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Log.Info($"Reset Password - Username: {account.Username}, Displayname: {account.Username}.");
                }
                else
                {
                    DialogBox.Show($"Đặt lại mật khẩu cho tài khoản {txbDisplayName.Text} thất bại.");
                }

            }
        }

        private void ButtonPrintBill_Click(object sender, RoutedEventArgs e)
        {
            var billId = Convert.ToInt32(lsvBills.SelectedItem.GetType().GetProperties()[0].GetValue(lsvBills.SelectedItem));

            new wPrintBill(billId).ShowDialog();
        }

        private void ButtonInsertAccount_Click(object sender, RoutedEventArgs e)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm 1 tài khoản mới?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var insertAccount = new wInsertAccount(Resources);
                insertAccount.Accounts = Accounts;
                insertAccount.ShowDialog();
            }
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            var billId = Convert.ToInt32(lsvBills.SelectedItem.GetType().GetProperties()[0].GetValue(lsvBills.SelectedItem));

            var billDetails = BillDetailBUS.Instance.GetListBillDetailByBillId(billId);
            ReportDataSource rds = new ReportDataSource("dsBillDetail", billDetails);

            //var data = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Bill WHERE Bill.Id = {billId}");
            var bill = BillBUS.Instance.GetBillById(billId);
            var bills = new List<Models.Bill> { bill };
            ReportDataSource rds2 = new ReportDataSource("dsBill", bills);

            var rpvPrintBill = sender as ReportViewer;
            rpvPrintBill.LocalReport.ReportEmbeddedResource = "GUI.Image.rpPrintBill.rdlc";
            rpvPrintBill.LocalReport.DataSources.Clear();
            rpvPrintBill.LocalReport.DataSources.Add(rds);
            rpvPrintBill.LocalReport.DataSources.Add(rds2);

            rpvPrintBill.RefreshReport();

            var dateCheckOut = Convert.ToDateTime(bill.DateCheckOut);
            rpvPrintBill.LocalReport.DisplayName = $"BillId_{billId}_" + dateCheckOut.ToString("ddMMyyy_hhmmss_tt");
        }

        private void tabReport_Selected(object sender, RoutedEventArgs e)
        {
            new wReport(Resources).ShowDialog();
        }
    }
}
