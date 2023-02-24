using Interfaces;
using Models;
using System.Data;

namespace SQLiteDataAccess
{
    public class BillDetailDAO : IBillDetailDAO
    {
        private BillDetailDAO() { }

        /// <summary>
        /// Lấy danh sách BillDetail dựa vào TableID của Bill.
        /// <br>Bill này phải là Bill chưa thanh toán (BillStatus = 0).</br>
        /// </summary>
        /// <param name="tableId">TableID của Bill.</param>
        /// <returns></returns>
        public List<BillDetail> GetListBillDetailByTableId(int tableId)
        {           
            string query = @"SELECT [Food].[Name] AS [FoodName],
                                    [FoodCategory].[Name] AS [CategoryName],
                                    [BillInfo].[FoodCount] AS [FoodCount],
                                    [Food].[Price] AS [Price],
                                    [FoodCount] * [Price] AS [TotalPrice]
                               FROM [Bill],
                                    [BillInfo],
                                    [Food],
                                    [FoodCategory]
                              WHERE [BillInfo].[BillID] = [Bill].[ID] AND 
                                    [BillInfo].[FoodID] = [Food].[ID] AND 
                                    [Food].[CategoryID] = [FoodCategory].[ID] AND 
                                    [Bill].[BillStatus] = 0 AND 
                                    [Bill].[TableID] = " + tableId;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            List<BillDetail> listBillDetail = new List<BillDetail>(data.Rows.Count);

            foreach (DataRow row in data.Rows)
            {
                BillDetail billDetail = new BillDetail(row);

                listBillDetail.Add(billDetail);
            }

            return listBillDetail;
        }

        /// <summary>
        /// Lấy danh sách BillDetail dựa vào ID của Bill.
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public List<BillDetail> GetListBillDetailByBillId(int billId)
        {           
            string query = @"SELECT [Food].[Name] AS [FoodName],
                                    [FoodCategory].[Name] AS [CategoryName],
                                    [BillInfo].[FoodCount] AS [FoodCount],
                                    [Food].[Price] AS [Price],
                                    [FoodCount] * [Price] AS [TotalPrice]
                               FROM [BillInfo],
                                    [Food],
                                    [FoodCategory]
                              WHERE [BillInfo].[FoodID] = [Food].[ID] AND 
                                    [Food].[CategoryID] = [FoodCategory].[ID] AND 
                                    [BillInfo].[BillID] = " + billId;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            List<BillDetail> listBillDetail = new List<BillDetail>(data.Rows.Count);

            foreach (DataRow row in data.Rows)
            {
                BillDetail billDetail = new BillDetail(row);

                listBillDetail.Add(billDetail);
            }

            return listBillDetail;
        }
    }
}
