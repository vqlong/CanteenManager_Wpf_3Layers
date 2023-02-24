using BUS;
using Microsoft.Reporting.WinForms;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for wReport.xaml
    /// </summary>
    public partial class wReport : Window
    {
        public wReport(ResourceDictionary resource)
        {
            Resources.MergedDictionaries.Add(resource);

            InitializeComponent();

            DataContext = this;
        }

        public static DateTime FirstDayInMonth { get; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public static DateTime LastDayInMonth { get; } = FirstDayInMonth.AddMonths(1).AddSeconds(-1);

        void LoadReport(DateTime fromDate, DateTime toDate)
        {
            var data = FoodBUS.Instance.GetRevenueByFoodAndDate(fromDate, toDate);
            ReportDataSource rds = new ReportDataSource("dsRevenueByFoodAndDate", data);

            var data2 = BillBUS.Instance.GetRevenueByMonth(fromDate, toDate);
            ReportDataSource rds2 = new ReportDataSource("dsRevenueByMonth", data2);

            rpvRevenue.LocalReport.ReportEmbeddedResource = "GUI.Image.rpRevenue.rdlc";
            rpvRevenue.LocalReport.DataSources.Clear();
            rpvRevenue.LocalReport.DataSources.Add(rds);
            rpvRevenue.LocalReport.DataSources.Add(rds2);
            rpvRevenue.RefreshReport();

        }

        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            if (dpkFromDate.SelectedDate != null && dpkToDate.SelectedDate != null)
                LoadReport((DateTime)dpkFromDate.SelectedDate, (DateTime)dpkToDate.SelectedDate);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (dpkFromDate.SelectedDate != null && dpkToDate.SelectedDate != null)
                LoadReport((DateTime)dpkFromDate.SelectedDate, (DateTime)dpkToDate.SelectedDate);
        }
    }
}
