using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Bill
    {
        public Bill()
        {

        }

        public Bill(int id, DateTime dateCheckIn, DateTime dateCheckOut, int tableId, int status, int discount = 0, double totalPrice = 0)
        {
            Id = id;
            DateCheckIn = dateCheckIn;
            DateCheckOut = dateCheckOut;
            TableId = tableId;
            Status = status;
            Discount = discount;
            TotalPrice = totalPrice;
        }

        public Bill(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            DateCheckIn = Convert.ToDateTime(row["DateCheckIn"]);
            //Dưới database, mặc định khi chạy proc thêm mới 1 Bill, DateCheckOut sẽ được gán là NULL => cần kiểm tra giá trị này khi xử lý trên C#
            DateCheckOut = row["DateCheckOut"] == DBNull.Value ? null : Convert.ToDateTime(row["DateCheckOut"]);
            TableId = (int)row["TableId"];
            Status = (int)row["BillStatus"];
            Discount = (int)row["Discount"];
            TotalPrice = (double)row["TotalPrice"];
        }


        private int id;
        private DateTime dateCheckIn;
        private DateTime? dateCheckOut;
        private int tableId;
        private int status;
        private int discount;
        private double totalPrice;

        public int Id { get => id; set => id = value; }
        public DateTime DateCheckIn { get => dateCheckIn; set => dateCheckIn = value; }
        public DateTime? DateCheckOut { get => dateCheckOut; set => dateCheckOut = value; }
        public int TableId { get => tableId; set => tableId = value; }
        public int Status { get => status; set => status = value; }
        public int Discount { get => discount; set => discount = value; }
        public double TotalPrice { get => totalPrice; set => totalPrice = value; }
        public virtual Table Table { get; set; }
        public virtual ICollection<BillInfo> BillInfos { get; set; } = new HashSet<BillInfo>();
    }
}
