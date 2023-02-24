using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BUS
{
    public class BillInfoBUS
    {
        private BillInfoBUS()
        {

        }
        private static readonly BillInfoBUS instance = new BillInfoBUS();
        public static BillInfoBUS Instance => instance;
        IBillInfoDAO BillInfo => Config.Container.Resolve<IBillInfoDAO>();
        public int InsertBillInfo(int billId, int foodId, int foodCount) => BillInfo.InsertBillInfo(billId, foodId, foodCount);
    }
}
