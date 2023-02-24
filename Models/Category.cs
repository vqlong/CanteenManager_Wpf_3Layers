using System.Data;

namespace Models
{
    /// <summary>
    /// Bảng FoodCategory của database.
    /// </summary>
    public class Category
    {
        public Category()
        {

        }

        public Category(int id, string name, UsingState categoryStatus)
        {
            Id = id;
            Name = name;
            CategoryStatus = categoryStatus;
        }

        /// <summary>
        /// Khởi tạo đối tượng từ 1 hàng của bảng FoodCategory.
        /// </summary>
        /// <param name="row"></param>
        public Category(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            Name = row["Name"].ToString();
            CategoryStatus = (UsingState)row["CategoryStatus"];
        }

        private int id;
        private string name;
        private UsingState categoryStatus;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public UsingState CategoryStatus { get => categoryStatus; set => categoryStatus = value; }
        public virtual ICollection<Food> Foods { get; set; } = new HashSet<Food>();
    }
}
