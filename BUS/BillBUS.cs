using Interfaces;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BUS
{
    public class BillBUS
    {
        private BillBUS()
        {

        }
        private static readonly BillBUS instance = new BillBUS();
        public static BillBUS Instance => instance;
        IBillDAO Bill => Config.Container.Resolve<IBillDAO>();
        public bool CheckOut(int billId, double totalPrice, int discount = 0) => Bill.CheckOut(billId, totalPrice, discount);
        public Bill GetBillById(int id) => Bill.GetBillById(id);
        public IEnumerable GetListBillByDate(DateTime fromDate, DateTime toDate) => Bill.GetListBillByDate(fromDate, toDate);
        public IEnumerable GetListBillByDateAndPage(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 10) => Bill.GetListBillByDateAndPage(fromDate, toDate, pageNumber, pageSize);
        public int GetNumberBillByDate(DateTime fromDate, DateTime toDate) => Bill.GetNumberBillByDate(fromDate, toDate);
        public object GetRevenueByMonth(DateTime fromDate, DateTime toDate) =>Bill.GetRevenueByMonth(fromDate, toDate);
        public int GetUnCheckBillIdByTableId(int tableId) => Bill.GetUnCheckBillIdByTableId(tableId);
        public int InsertBill(int tableId) => Bill.InsertBill(tableId);
    }
}
