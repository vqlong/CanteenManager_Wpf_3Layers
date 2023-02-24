using Models;

namespace Interfaces
{
    public interface IBillDetailDAO
    {
        List<BillDetail> GetListBillDetailByBillId(int billId);
        List<BillDetail> GetListBillDetailByTableId(int tableId);
    }
}