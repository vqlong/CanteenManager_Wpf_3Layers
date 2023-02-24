using System.Data;

namespace Models
{
    public class Food
    {
        public Food()
        {

        }

        public Food(int id, string name, int categoryId, double price, UsingState foodStatus)
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            Price = price;
            FoodStatus = foodStatus;
        }

        /// <summary>
        /// Khởi tạo đối tượng từ 1 hàng của bảng Food.
        /// </summary>
        /// <param name="row"></param>
        public Food(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            Name = row["Name"].ToString();
            CategoryId = (int)row["CategoryId"];
            Price = (double)row["Price"];
            FoodStatus = (UsingState)row["FoodStatus"];
        }

        private int id;
        private string name;
        private int categoryId;
        private double price;
        private UsingState foodStatus;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int CategoryId { get => categoryId; set => categoryId = value; }
        public double Price { get => price; set => price = value; }
        public UsingState FoodStatus { get => foodStatus; set => foodStatus = value; }
        public virtual Category Category { get; set; }
        public virtual ICollection<BillInfo> BillInfos { get; set; } = new HashSet<BillInfo>();
    }
}
