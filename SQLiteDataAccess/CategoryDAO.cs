using Interfaces;
using Models;
using System.Data;

namespace SQLiteDataAccess
{
    public class CategoryDAO : ICategoryDAO
    {
        private CategoryDAO() { }
        private static readonly CategoryDAO instance = new CategoryDAO();
        public static CategoryDAO Instance => instance;

        /// <summary>
        /// Lấy tất cả dữ liệu trong bảng FoodCategory từ database để tạo các đối tượng Category và đưa vào danh sách.
        /// </summary>
        /// <returns></returns>
        public List<Category> GetListCategory()
        {
            List<Category> listCategory = new List<Category>();

            string query = "SELECT * FROM FoodCategory";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                Category category = new Category(row);
                category.Foods = FoodDAO.Instance.GetListFoodByCategoryId(category.Id);
                listCategory.Add(category);
            }

            return listCategory;
        }

        /// <summary>
        /// Trả về list các Category có trạng thái là đang phục vụ (UsingState.Serving).
        /// </summary>
        /// <returns></returns>
        public List<Category> GetListCategoryServing()
        {
            List<Category> listCategory = new List<Category>();

            string query = "SELECT * FROM FoodCategory WHERE CategoryStatus = 1";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                Category category = new Category(row);
                category.Foods = FoodDAO.Instance.GetListFoodByCategoryId(category.Id).Where(f => f.FoodStatus == UsingState.Serving).ToList();

                listCategory.Add(category);
            }

            return listCategory;
        }

        /// <summary>
        /// Tạo 1 đối tượng Category dựa vào Id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Category GetCategoryById(int categoryId)
        {
            string query = "SELECT * FROM FoodCategory WHERE ID = " + categoryId;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            Category category = new Category(data.Rows[0]);
            category.Foods = FoodDAO.Instance.GetListFoodByCategoryId(category.Id).Where(f => f.FoodStatus == UsingState.Serving).ToList();
            return category;
        }

        public Category InsertCategory(string name)
        {
            string query = $"INSERT INTO FoodCategory(Name) VALUES( @name )";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] {name});

            if (result == 1)
            {
                query = "SELECT * FROM FoodCategory WHERE Id = (SELECT Max(Id) FROM FoodCategory) ;";

                DataTable data = DataProvider.Instance.ExecuteQuery(query);

                if (data.Rows.Count > 0)
                {
                    Category category = new Category(data.Rows[0]);

                    return category;
                }
                return null;
            }

            return null;
        }

        public bool UpdateCategory(int id, string name, UsingState categoryStatus)
        {
            string query = $"UPDATE FoodCategory SET Name = @name, CategoryStatus = @categoryStatus WHERE ID = @id ";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name, (int)categoryStatus, id });

            if (result == 1)
            {
                query = $"UPDATE Food SET FoodStatus = @categoryStatus WHERE CategoryId = @categoryId ";
                result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { (int)categoryStatus, id });
                if(result >= 1) return true;
                return false;
            }
            else
            {
                return false;
            }
            
        }

        public bool DeleteCategory(int id)
        {
            string query = @$"UPDATE [FoodCategory]
                                 SET [CategoryStatus] = 0 
                               WHERE [FoodCategory].[Id] = @id ;";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });

            if (result == 1)
            {
                query = $"UPDATE Food SET FoodStatus = 0 WHERE CategoryId = @categoryId ";
                result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });
                if (result >= 1) return true;

                return false;
            }

            return false;
        }
    }
}
