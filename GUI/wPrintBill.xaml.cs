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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for wPrintBill.xaml
    /// </summary>
    public partial class wPrintBill : Window
    {
        public wPrintBill(int billId)
        {
            InitializeComponent();

            this.billId = billId;
        }
        int billId;
        private void ReportViewer_Load(object sender, EventArgs e)
        {
            BindingSource bindingSource = new BindingSource() { DataSource = BillDetailBUS.Instance.GetListBillDetailByBillId(billId) };
            ReportDataSource rds = new ReportDataSource("dsBillDetail", bindingSource);

            //var data = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Bill WHERE Bill.Id = {billId}");
            var bill = BillBUS.Instance.GetBillById(billId);
            var data = new List<Models.Bill> { bill };
            ReportDataSource rds2 = new ReportDataSource("dsBill", data);

            var rpvPrintBill = sender as ReportViewer;
            rpvPrintBill.LocalReport.ReportEmbeddedResource = "GUI.Image.rpPrintBill.rdlc";
            rpvPrintBill.LocalReport.DataSources.Clear();
            rpvPrintBill.LocalReport.DataSources.Add(rds);
            rpvPrintBill.LocalReport.DataSources.Add(rds2);

            rpvPrintBill.RefreshReport();

            var dateCheckOut = Convert.ToDateTime(bill.DateCheckOut);
            rpvPrintBill.LocalReport.DisplayName = $"BillId_{billId}_" + dateCheckOut.ToString("ddMMyyy_hhmmss_tt");
        }
    }
}
