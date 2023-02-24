using Models;
using System.Data;
using Interfaces;

namespace SQLiteDataAccess
{
    public class TableDAO : ITableDAO
    {
        private TableDAO() { }

        /// <summary>
        /// Lấy những bàn có thể được sử dụng từ database để tạo các đối tượng Table và đưa vào danh sách. 
        /// </summary>
        /// <returns></returns>
        public List<Table> GetListTableUsing()
        {
            string query = "SELECT * FROM TableFood WHERE UsingState = 1;";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            List<Table> listTable = new List<Table>();

            foreach (DataRow row in data.Rows)
            {
                Table table = new Table(row);

                listTable.Add(table);
            }

            return listTable;
        }

        /// <summary>
        /// Lấy tất cả các bàn hiện có trong database.
        /// </summary>
        /// <returns></returns>
        public List<Table> GetListTable()
        {
            string query = "SELECT * FROM TableFood;";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            List<Table> listTable = new List<Table>();

            foreach (DataRow row in data.Rows)
            {
                Table table = new Table(row);

                listTable.Add(table);
            }

            return listTable;
        }

        /// <summary>
        /// Tạo 1 đối tượng Table dựa vào ID.
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public Table GetTableById(int tableID)
        {
            string query = "SELECT * FROM TableFood WHERE ID = " + tableID;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            Table table = new Table(data.Rows[0]);

            return table;
        }

        /// <summary>
        /// Chuyển bàn, bằng cách tráo đổi TableId của 2 Bill trên mỗi bàn.
        /// </summary>
        /// <param name="firstTableId"></param>
        /// <param name="secondTableId"></param>
        public bool SwapTable(int firstTableId, int secondTableId)
        {
            var firstBillId = BillDAO.Instance.GetUnCheckBillIdByTableId(firstTableId);
            var secondBillId = BillDAO.Instance.GetUnCheckBillIdByTableId(secondTableId);

            string query = "";

            //Nếu bàn thứ nhất được chọn có người thì mới tiến hành chuyển
            if (firstBillId > 0)
            {
                query += $"UPDATE Bill SET TableID = {secondTableId} WHERE ID = {firstBillId};";

                if (secondBillId > 0) query += $"UPDATE Bill SET TableID = {firstTableId} WHERE ID = {secondBillId};";
            }

            var result = DataProvider.Instance.ExecuteNonQuery(query);

            if (result > 0) return true;

            return false;
        }

        /// <summary>
        /// Gộp bàn, bằng cách chuyển các món ăn trên bàn này vào bàn kia.
        /// </summary>
        /// <param name="firstTableId"></param>
        /// <param name="secondTableId"></param>
        public bool CombineTable(int firstTableId, int secondTableId)
        {
            var firstBillId = BillDAO.Instance.GetUnCheckBillIdByTableId(firstTableId);
            var secondBillId = BillDAO.Instance.GetUnCheckBillIdByTableId(secondTableId);
            
            //Nếu có bàn không có người thì thôi
            if (firstBillId <= 0 || secondBillId <= 0) return false;

            string query = $"UPDATE BillInfo SET BillID = {secondBillId} WHERE BillID = {firstBillId};";
            var result = DataProvider.Instance.ExecuteNonQuery(query);
            if (result <= 0) return false;

            //Xoá cái Bill thứ 1 (lúc này tất cả thức ăn đã chuyển sang Bill thứ 2)
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Bill WHERE ID = {firstBillId};");

            //Khi gộp bàn sẽ xuất hiện các món trùng lặp với nhau
            //Lấy ra các FoodID với số lần trùng lặp
            query = $"SELECT max(ID) AS 'MaxID', FoodID, count(*) AS 'Count', sum(FoodCount) AS 'TotalFoodCount' FROM BillInfo WHERE BillID = {secondBillId} GROUP BY FoodID;";
            var table = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                var maxID = Convert.ToInt32(row["MaxID"]);
                var foodID = Convert.ToInt32(row["FoodID"]);
                var count = Convert.ToInt32(row["Count"]);
                var totalFoodCount = Convert.ToInt32(row["TotalFoodCount"]);
                //Trường hợp count > 1 tức là món này bị trùng nhau, xuất hiện hơn 1 lần
                if (count > 1)
                {
                    //Lấy ra max ID của món này để tí nữa giữ lại và update, các ID khác xoá hết cho khỏi trùng nhau
                    DataProvider.Instance.ExecuteNonQuery($"UPDATE BillInfo SET FoodCount = {totalFoodCount} WHERE BillID = {secondBillId} AND FoodID = {foodID} AND ID = {maxID};");

                    DataProvider.Instance.ExecuteNonQuery($"DELETE FROM BillInfo WHERE BillID = {secondBillId} AND FoodID = {foodID} AND ID != {maxID};");
                 
                }
            }

            return true;
        }

        public Table InsertTable()
        {
            string query = $"INSERT INTO TableFood(Name) VALUES('Bàn ' || CAST((SELECT COUNT(*) FROM TableFood) + 1 AS NVARCHAR(100)));";

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            if (result == 1)
            {
                query = "SELECT * FROM TableFood WHERE Id = (SELECT Max(Id) FROM TableFood) ;";

                DataTable data = DataProvider.Instance.ExecuteQuery(query);

                if (data.Rows.Count > 0)
                {
                    Table table = new Table(data.Rows[0]);

                    return table;
                }
                return null;
            }

            return null;
        }

        public bool UpdateTable(int id, string name, UsingState usingState)
        {
            string query = $"UPDATE TableFood SET Name = @name , UsingState = @usingState WHERE ID = @id ";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] {name, (int)usingState, id});

            if (result == 1) return true;

            return false;
        }

        public bool DeleteTable(int id)
        {
            string query = @$"UPDATE [TableFood]
                                 SET [UsingState] = 0 
                               WHERE [TableFood].[Id] = @id ;";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });

            if (result == 1) return true;

            return false;
        }
    }
}
