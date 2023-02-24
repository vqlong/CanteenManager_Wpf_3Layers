using Interfaces;

namespace SQLiteDataAccess
{
    public class BillInfoDAO : IBillInfoDAO
    {
        private BillInfoDAO() { }

        public int InsertBillInfo(int billId, int foodId, int foodCount)
        {
            //Kiểm tra xem cái Bill này đã thanh toán chưa, chưa thanh toán mới được thêm BillInfo vào
            var billStatus = Convert.ToInt32(DataProvider.Instance.ExecuteScalar($"SELECT BillStatus FROM Bill WHERE Id = {billId};"));
            if (billStatus == 1) return -1;

            //Kiểm tra xem cái BillInfo này đã có chưa
            var countBillInfo = Convert.ToInt32(DataProvider.Instance.ExecuteScalar($"SELECT COUNT(*) FROM BillInfo WHERE BillInfo.BillId = {billId} AND BillInfo.FoodId = {foodId};"));          
            
            //Nếu chưa có thì thêm mới
            if (countBillInfo == 0)
            {
                if(foodCount > 0)
                {
                    DataProvider.Instance.ExecuteNonQuery($"INSERT INTO BillInfo(BillID, FoodID, FoodCount) VALUES({billId}, {foodId}, {foodCount});");
                    return Convert.ToInt32(DataProvider.Instance.ExecuteScalar("SELECT MAX(Id) FROM BillInfo;"));
                }
                else
                {
                    return -1;
                }
                
            }

            //Nếu có rồi thì update số lượng món đã gọi
            var id = Convert.ToInt32(DataProvider.Instance.ExecuteScalar($"SELECT Id FROM BillInfo WHERE BillInfo.BillId = {billId} AND BillInfo.FoodId = {foodId};"));
            var currentFoodCount = Convert.ToInt32(DataProvider.Instance.ExecuteScalar($"SELECT FoodCount FROM BillInfo WHERE BillInfo.Id = {id};"));
            var newFoodCount = currentFoodCount + foodCount;
            
            //Theo thiết kế @foodCount truyền vào có thể âm, nếu @newFoodCount <= 0 thì xoá món đó khỏi hoá đơn
            if (newFoodCount <= 0)
            {
                DataProvider.Instance.ExecuteNonQuery($"DELETE FROM BillInfo WHERE BillInfo.Id = {id};");

                //Sau mỗi lần xoá BillInfo, đếm xem cái Bill này còn BillInfo nào không, nếu không còn cái nào thì xoá luôn Bill
                var count = Convert.ToInt32(DataProvider.Instance.ExecuteScalar($"SELECT COUNT(*) FROM BillInfo WHERE BillInfo.BillId = {billId};"));
                
                if (count == 0) DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Bill WHERE Id = {billId};");

                return -1;
                
            }

            DataProvider.Instance.ExecuteNonQuery($"UPDATE BillInfo SET FoodCount = {newFoodCount} WHERE BillInfo.Id = {id};");
            return id;
        }
    }
}
