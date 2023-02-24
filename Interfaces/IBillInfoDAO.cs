namespace Interfaces
{
    public interface IBillInfoDAO
    {
        int InsertBillInfo(int billId, int foodId, int foodCount);
    }
}