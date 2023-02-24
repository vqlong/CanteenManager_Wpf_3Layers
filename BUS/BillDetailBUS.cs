using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BUS
{
    public class BillDetailBUS
    {
        private BillDetailBUS()
        {

        }
        private static readonly BillDetailBUS instance = new BillDetailBUS();
        public static BillDetailBUS Instance => instance;
        IBillDetailDAO BillDetail => Config.Container.Resolve<IBillDetailDAO>();
        public List<BillDetail> GetListBillDetailByBillId(int billId) => BillDetail.GetListBillDetailByBillId(billId);
        public List<BillDetail> GetListBillDetailByTableId(int tableId) => BillDetail.GetListBillDetailByTableId(tableId);
    }
}
