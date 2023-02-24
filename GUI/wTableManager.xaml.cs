using BUS;
using Help;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using WpfLibrary;
using WpfLibrary.UserControls;
using Table = Models.Table;

namespace GUI
{
    /// <summary>
    /// Interaction logic for wTableManager.xaml
    /// </summary>
    public partial class wTableManager : Window
    {
        public wTableManager()
        {
            InitializeComponent();

            DataContext = this;
        }
        public static RoutedCommand Checkout { get; } = new RoutedCommand("Checkout", typeof(wTableManager));
        public static RoutedCommand SwapTable { get; } = new RoutedCommand("SwapTable", typeof(wTableManager));
        public static RoutedCommand CombineTable { get; } = new RoutedCommand("CombineTable", typeof(wTableManager));
        wMedia media;
        public static readonly DependencyProperty LoginAccountProperty = DependencyProperty.Register("LoginAccount", typeof(Account), typeof(wTableManager));
        public Account LoginAccount { get => (Account)GetValue(LoginAccountProperty); set => SetValue(LoginAccountProperty, value); }
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>(CategoryBUS.Instance.GetListCategory());
        public ObservableCollection<Table> Tables { get; } = new ObservableCollection<Table>(TableBUS.Instance.GetListTable());
        public ObservableCollection<NamedBrush> Brushes { get; } = new ObservableCollection<NamedBrush>();
        public ObservableCollection<FontFamily> FontFamilies { get; } = new ObservableCollection<FontFamily>(Fonts.SystemFontFamilies);
        public ObservableCollection<Skin> Skins { get; } = new ObservableCollection<Skin>();
        CategoryBUS Category => CategoryBUS.Instance;
        TableBUS Table => TableBUS.Instance;
        BillBUS Bill => BillBUS.Instance;

        private void LoadFonts()
        {
            ResourceDictionary dictionary = Application.Current.Resources.MergedDictionaries[2];
            foreach (var key in dictionary.Keys)
            {
                FontFamilies.Add((FontFamily)dictionary[key]);
            }
        }

        private void LoadBrushes()
        {
            ResourceDictionary dictionary = Application.Current.Resources.MergedDictionaries[6];
            foreach (var key in dictionary.Keys)
            {
                Brushes.Add(new NamedBrush(key.ToString(), (Brush)dictionary[key]));
            }

            var infoes = typeof(Colors).GetProperties();
            foreach (var info in infoes)
            {
                Brushes.Add(new NamedBrush(info.Name, new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Name))));
            }
        }

        private void MenuItemLogout_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlayMedia_Click(object sender, RoutedEventArgs e)
        {
            if (media == null)
            {
                media = new wMedia();

                txblMediaPosition.SetBinding(TextBlock.TextProperty, new Binding
                {
                    Source = media,
                    Path = new PropertyPath("TextPosition")
                });

                sliderMediaStatus.SetBinding(RangeBase.MinimumProperty, new Binding
                {
                    Source = media.SeekBar,
                    Path = new PropertyPath(RangeBase.MinimumProperty)
                });
                sliderMediaStatus.SetBinding(RangeBase.MaximumProperty, new Binding
                {
                    Source = media.SeekBar,
                    Path = new PropertyPath(RangeBase.MaximumProperty)
                });
                sliderMediaStatus.SetBinding(RangeBase.ValueProperty, new Binding
                {
                    Source = media.SeekBar,
                    Path = new PropertyPath(RangeBase.ValueProperty)
                });

                Storyboard storyboard = (Storyboard)TryFindResource("Storyboard.Equalizer");
                //media sẽ điều khiển equalizer cho cả 2 window
                media.StoryboardEqualizer = storyboard;
            }
            media.Show();
            media.Activate();
        }

        private void cbSkins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox comboBox)
            {
                if(comboBox.SelectedItem is Skin skin)
                {
                    foreach (var key in skin.Settings.Keys)
                    {
                        var obj = FindName(key);
                        if (obj is Selector selector) SetSelectedItem(selector, skin.Settings[key]);
                    }
                }
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Tắt hẳn media
            if(media != null)
            {
                media.ShouldClose = true;
                media.Close();
            }

            //Lưu tên skin hiện tại
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Skins");
            File.WriteAllText($@"{Environment.CurrentDirectory}\Skins\CurrentSkin.txt", cbSkins.SelectedValue.ToString());

            Log.Info($"Logout Account - Username: {LoginAccount.Username}, Displayname: {LoginAccount.DisplayName}.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("pack://application:,,,/PresentationFramework.Royale;component/themes/Royale.NormalColor.xaml");
            var dictionary = new ResourceDictionary { Source = uri };
            foreach (var key in dictionary.Keys)
            {
                //Lấy ra style cho scrollbar và thêm vào resource của window
                if (dictionary[key] is Style style && style.TargetType == typeof(ScrollBar))
                {
                    Resources.Add(key, style);
                    break;
                }
            }

            //Xoá bỏ các food đã dừng bán
            foreach (var category in Categories)
            {
                foreach (var food in category.Foods)
                {
                    if (food.FoodStatus == UsingState.StopServing) category.Foods.Remove(food);
                }
            }

            //Xoá bỏ các food đã dừng bán mỗi khi Categories được cập nhật
            Categories.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null || e.NewItems.Count <= 0) return;
                var category = e.NewItems[0] as Category;
                foreach (var food in category.Foods)
                {
                    if (food.FoodStatus == UsingState.StopServing) category.Foods.Remove(food);
                }
            };

            var categoriesViewSource = new CollectionViewSource();
            categoriesViewSource.Source = Categories;
            //Lọc, giữ lại các category đang bán
            categoriesViewSource.View.Filter = (obj) => (obj as Category).CategoryStatus == UsingState.Serving;
            cbCategories.ItemsSource = categoriesViewSource.View;

            var tablesViewSource = new CollectionViewSource();
            tablesViewSource.Source = Tables;
            tablesViewSource.View.Filter = (obj) => (obj as Table).UsingState == UsingState.Serving;
            lsvTables.ItemsSource = tablesViewSource.View;

            LoadBrushes();
            LoadFonts();
            LoadSkins();
            LoadCurrentSkin();

        }

        private void LoadCurrentSkin()
        {
            var currentSkin = @$"{Environment.CurrentDirectory}\Skins\CurrentSkin.txt";
            if (File.Exists(currentSkin))
            {
                var skinName = File.ReadAllText(currentSkin);
                if (Skins.Any(skin => skin.Name.Equals(skinName)))
                {
                    cbSkins.SelectedValue = skinName;
                    return;
                }
            }
            cbSkins.SelectedValue = "Default";
        }

        private void LoadSkins()
        {
            using var stream = Application.GetResourceStream(new Uri("Image\\Classic.json", UriKind.Relative)).Stream;
            var skin = JsonSerializer.Deserialize<Skin>(stream);
            Skins.Add(skin);

            using var stream2 = Application.GetResourceStream(new Uri("Image\\Dark.json", UriKind.Relative)).Stream;
            var skin2 = JsonSerializer.Deserialize<Skin>(stream2);
            Skins.Add(skin2);

            using var stream3 = Application.GetResourceStream(new Uri("Image\\Default.json", UriKind.Relative)).Stream;
            var skin3 = JsonSerializer.Deserialize<Skin>(stream3);
            Skins.Add(skin3);

            using var stream4 = Application.GetResourceStream(new Uri("Image\\Gradient.json", UriKind.Relative)).Stream;
            var skin4 = JsonSerializer.Deserialize<Skin>(stream4);
            Skins.Add(skin4);

            var customSkinPath = @$"{Environment.CurrentDirectory}\Skins";
            if (Directory.Exists(customSkinPath))
            {
                var files = Directory.EnumerateFiles(customSkinPath, "*.json");
                foreach (var file in files)
                {
                    //using FileStream stream5 = new FileStream(file, FileMode.Open);
                    //stream5.Position = 0;
                    //var customSkin = JsonSerializer.Deserialize<Skin>(stream);

                    var json = File.ReadAllText(file);
                    var customSkin = JsonSerializer.Deserialize<Skin>(json);
                    Skins.Add(customSkin);
                }
            }
        }

        private void SetSelectedItem(Selector selector, string name)
        {
            foreach (var item in selector.Items)
            {
                if(item.ToString().Equals(name))
                {
                    selector.SelectedIndex = selector.Items.IndexOf(item);
                    return;
                }
            }
        }

        private void MenuItemSaveCurrentSkin_Click(object sender, RoutedEventArgs e)
        {
            if (Skins.Any(skin => skin.Name.Equals(txbSkinName.Text)))
            {
                DialogBox.Show("1 Skin với tên này đã tồn tại, hãy chọn 1 tên khác.");
                return;
            }
            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                { cbTextColor.Name, cbTextColor.SelectedItem.ToString()},
                { cbBackground.Name, cbBackground.SelectedItem.ToString()},
                { cbControlBackground.Name, cbControlBackground.SelectedItem.ToString()},
                { cbBorderColor.Name, cbBorderColor.SelectedItem.ToString()},
                { cbBillColor.Name, cbBillColor.SelectedItem.ToString()},
                { cbSelectedTableColor.Name, cbSelectedTableColor.SelectedItem.ToString()},
                { cbTableBorderColor.Name, cbTableBorderColor.SelectedItem.ToString()},
                { cbUsingTableColor.Name, cbUsingTableColor.SelectedItem.ToString()},
                { cbUsingTableNameColor.Name, cbUsingTableNameColor.SelectedItem.ToString()},
                { cbEmptyTableColor.Name, cbEmptyTableColor.SelectedItem.ToString()},
                { cbEmptyTableNameColor.Name, cbEmptyTableNameColor.SelectedItem.ToString()},
                { cbFontFamilies.Name, cbFontFamilies.SelectedItem.ToString()},
            };
            var skin = new Skin(txbSkinName.Text, settings);
            var path = $@"{Environment.CurrentDirectory}\Skins";
            Directory.CreateDirectory(path);
            using var stream = File.Create($@"{path}\{skin.Name}.json");
            JsonSerializer.Serialize(stream, skin);
            //stream.Position = 0;
            DialogBox.Show($"Skin {skin.Name}.json đã được lưu tại {path}.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

        }

        private void MenuItemAccountProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new wAccountProfile(Resources);
            profile.SetBinding(BackgroundProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(BackgroundProperty)
            });
            profile.SetBinding(BorderBrushProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(BorderBrushProperty)
            });
            profile.LoginAccount = LoginAccount;

            profile.ShowDialog();
        }

        private void ButtonAddFood_Click(object sender, RoutedEventArgs e)
        {
            if (lsvTables.SelectedItem == null)
            {
                DialogBox.Show("Chưa có bàn ăn nào được chọn.");
                return;
            }

            int foodCount = (int)nmFoodCount.Value;
            if (foodCount == 0) return;
            var tableId = (lsvTables.SelectedItem as Table).Id;
            var billId = Bill.GetUnCheckBillIdByTableId(tableId);
            int foodId = (cbFoods.SelectedItem as Food).Id;
            

            if (billId == -1)
            {
                if (foodCount < 0) return;

                var newBillId = Bill.InsertBill(tableId);

                Log.Info($"Insert Bill - Id: {newBillId}.");

                BillInfoBUS.Instance.InsertBillInfo(newBillId, foodId, foodCount);
            }
            else
            {
                BillInfoBUS.Instance.InsertBillInfo(billId, foodId, foodCount);
            }

            ReloadTable(tableId);
        }

        private void ReloadTable(int tableId)
        {
            var oldTable = Tables.FirstOrDefault(t => t.Id == tableId);
            if (oldTable == null) return;
            var index = Tables.IndexOf(oldTable);
            Tables.RemoveAt(index);
            var newTable = Table.GetTableById(tableId);
            Tables.Insert(index, newTable);
            lsvTables.SelectedIndex = index;
            cbTables.SelectedIndex = index;
        }

        private void Checkout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvBill != null && lsvBill.Items.Count > 0) e.CanExecute = true;
        }

        private void Checkout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            double billTotalPrice;
            if (double.TryParse(txblTotalPrice.Tag.ToString(), out billTotalPrice) == false) return;

            Table table = lsvTables.SelectedItem as Table;

            int billId = Bill.GetUnCheckBillIdByTableId(table.Id);

            if (billId != -1)
            {
                int discount = (int)nmDiscount.Value;

                double billFinalPrice = billTotalPrice - billTotalPrice * discount / 100;

                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("vi-vn");

                if (DialogBox.Show($"Bạn có muốn thanh toán hoá đơn cho\n{table.Name}?\nTổng tiền:\t {txblTotalPrice.Text}\nGiảm giá:\t {discount}%\nPhải trả:\t\t {billFinalPrice.ToString("c1", culture)}", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
                {
                    var result = Bill.CheckOut(billId, billFinalPrice, discount);

                    Log.Info($"Checkout Bill - Id: {billId}, Result: {result}.");

                    if (result == false)
                    {
                        DialogBox.Show("Có lỗi trong quá trình thanh toán.\nThanh toán thất bại.");
                        return;
                    }

                    if (DialogBox.Show("Bạn có muốn in hoá đơn?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
                    {
                        new wPrintBill(billId).ShowDialog();
                    }
                }

                ReloadTable(table.Id);
            }
        }

        private void SwapTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvTables != null && lsvTables.SelectedItem != null && cbTables != null && cbTables.SelectedItem != null)
            {
                Table table1 = lsvTables.SelectedItem as Table;
                Table table2 = cbTables.SelectedItem as Table;
                if (table1.Status == "Có người" && table1.Id != table2.Id) e.CanExecute = true;
            }
        }

        private void SwapTable_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Table table1 = lsvTables.SelectedItem as Table;
            Table table2 = cbTables.SelectedItem as Table;

            if (DialogBox.Show($"Bạn có thực sự muốn chuyển {table1.Name} tới {table2.Name}?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
            {
                Table.SwapTable(table1.Id, table2.Id);
                ReloadTable(table1.Id);
                ReloadTable(table2.Id);
            }
        }

        private void CombineTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lsvTables != null && lsvTables.SelectedItem != null && cbTables != null && cbTables.SelectedItem != null)
            {
                Table table1 = lsvTables.SelectedItem as Table;
                Table table2 = cbTables.SelectedItem as Table;
                if (table1.Status == "Có người" && table2.Status == "Có người" && table1.Id != table2.Id) e.CanExecute = true;
            }
        }

        private void CombineTable_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Table table1 = lsvTables.SelectedItem as Table;
            Table table2 = cbTables.SelectedItem as Table;

            if (DialogBox.Show($"Bạn có thực sự muốn gộp {table1.Name} vào {table2.Name}?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
            {
                Table.CombineTable(table1.Id, table2.Id);
                ReloadTable(table1.Id);
                ReloadTable(table2.Id);
            }
        }

        private void MenuItemAdmin_Click(object sender, RoutedEventArgs e)
        {
            var admin = new wAdmin(Resources);
            admin.SetBinding(BackgroundProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(BackgroundProperty)
            });
            admin.SetBinding(BorderBrushProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(BorderBrushProperty)
            });
            admin.LoginAccount = LoginAccount;
            admin.Categories = Categories;
            admin.Tables = Tables;
            admin.ShowDialog();
        }

        bool isAscending;
        GridViewColumnHeaderSortingAdorner sortingAdorner;
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = (GridViewColumnHeader)sender;
            var column = header.Column;
            //Mỗi GridViewColumn đã được attach 1 số double để xác định nó hiển thị property nào của model nào
            var columnId = AttachedManager.GetDouble(column);
            var content = header.Content as string;
            var propertyName = "";
            //listview bill
            if (columnId == 1) propertyName = "FoodName";
            if (columnId == 2) propertyName = "CategoryName";
            if (columnId == 3) propertyName = "FoodCount";
            if (columnId == 4) propertyName = "Price";
            if (columnId == 5) propertyName = "TotalPrice";
            //listview food (window Admin)
            if (columnId == 6) propertyName = "Id";
            if (columnId == 7) propertyName = "Name";
            if (columnId == 8) propertyName = "CategoryId";
            if (columnId == 9) propertyName = "Price";
            if (columnId == 10) propertyName = "FoodStatus";
            //listview category (window Admin)
            if (columnId == 11) propertyName = "Id";
            if (columnId == 12) propertyName = "Name";
            if (columnId == 13) propertyName = "CategoryStatus";
            //listview bill (window Admin)
            if (columnId == 14) propertyName = "Id";
            if (columnId == 15) propertyName = "DateCheckIn";
            if (columnId == 16) propertyName = "DateCheckOut";
            if (columnId == 17) propertyName = "TableName";
            if (columnId == 18) propertyName = "Discount";
            if (columnId == 19) propertyName = "TotalPrice";
            //listview table (window Admin)
            if (columnId == 20) propertyName = "Id";
            if (columnId == 21) propertyName = "Name";
            if (columnId == 22) propertyName = "Status";
            if (columnId == 23) propertyName = "UsingState";
            //listview account (window Admin)
            if (columnId == 24) propertyName = "Username";
            if (columnId == 25) propertyName = "DisplayName";
            if (columnId == 26) propertyName = "Type";

            if (propertyName == "") throw new Exception("Tên cột không phù hợp");

            var ListView = (ItemsControl)AttachedManager.GetTag(column);
            var view = CollectionViewSource.GetDefaultView(ListView.ItemsSource);
            if (isAscending)
            {
                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Descending));
                isAscending = false;
            }
            else
            {
                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
                isAscending = true;
            }

            if (sortingAdorner != null) ((AdornerLayer)sortingAdorner.Parent).Remove(sortingAdorner);
            sortingAdorner = new GridViewColumnHeaderSortingAdorner(header, isAscending);
            AdornerLayer.GetAdornerLayer(header).Add(sortingAdorner);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var processStartInfor = new ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true
            };

            Process.Start(processStartInfor);
        }
    }
}
