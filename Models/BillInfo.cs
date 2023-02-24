using System.Data;

namespace Models
{
    public class BillInfo
    {
        public BillInfo()
        {

        }

        public BillInfo(int id, int billId, int foodId, int foodCount)
        {
            Id = id;
            BillId = billId;
            FoodId = foodId;
            FoodCount = foodCount;
        }

        public BillInfo(DataRow row)
        {
            Id = (int)row["Id"];
            BillId = (int)row["BillId"];
            FoodId = (int)row["FoodId"];
            FoodCount = (int)row["FoodCount"];
        }

        private int id;
        private int billId;
        private int foodId;
        private int foodCount;

        public int Id { get => id; set => id = value; }
        public int BillId { get => billId; set => billId = value; }
        public int FoodId { get => foodId; set => foodId = value; }
        public int FoodCount { get => foodCount; set => foodCount = value; }
        public virtual Bill Bill { get; set; }  
        public virtual Food Food { get; set; }
    }
}
