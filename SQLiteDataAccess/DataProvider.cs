using Interfaces;
using System.Data;
using System.Data.SQLite;

namespace SQLiteDataAccess
{
    /// <summary>
    /// Chứa các phương thức làm việc với database của SQLite.
    /// </summary>
    public class DataProvider : IDataProvider
    {
        private DataProvider() { }

        private static readonly DataProvider instance = new DataProvider();
        public static DataProvider Instance => instance;

        public string Name => "ADO.NET-SQLite";

        private string connectionString = "Data Source = localdb.db; foreign keys=true";

        /// <summary>
        /// Kiểm tra kết nối.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối.</param>
        /// <returns>
        /// true, nếu thành công, đồng thời gán chuỗi kết nối này cho biến connectionString của SQLiteDataProvider.
        /// <br>nếu thất bại, thông báo lỗi và trả về false.</br>
        /// </returns>
        public bool TestConnection(string connectionString)
        {          

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand("SELECT 1", connection);

                    if ((long)command.ExecuteScalar() == 1)
                    {
                        this.connectionString = connectionString;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Trả về 1 bảng kết quả khi chạy câu truy vấn.
        /// </summary>
        /// <param name="query">Câu truy vấn.</param>
        /// <param name="parameter">Mảng các giá trị truyền cho các parameter @xyz... </param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable data = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

                if (parameter != null)
                {
                    //cắt nhỏ câu truy vấn để lấy ra phần parameter dạng @abcxyz
                    string[] paraList = query.Split(' ');
                    int i = 0;
                    foreach (string item in paraList)
                    {
                        //nếu chứa '@' => nó là parameter
                        if (item.Contains('@'))
                        {
                            //xoá dấu "," sau đít tham số
                            string para = item.Replace(",", "");
                            //thêm vào danh sách tên parameter và giá trị tương ứng được truyền cho nó, nếu nó null thì truyền vào DBNull.Value
                            command.Parameters.AddWithValue(para, parameter[i] ?? DBNull.Value);
                            //chỉ tăng i mỗi khi tìm được tên 1 parameter
                            i++;
                        }
                    }

                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);

            }

            return data;
        }

        /// <summary>
        /// Trả về số số dòng áp dụng thành công khi chạy câu truy vấn.
        /// </summary>
        /// <param name="query">Câu truy vấn.</param>
        /// <param name="parameter">Mảng các parameter.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            //tạo biến chứa số dòng áp dụng thành công khi chạy câu truy vấn
            int data = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

                if (parameter != null)
                {
                    //cắt nhỏ câu truy vấn để lấy ra phần parameter dạng @abcxyz
                    string[] paraList = query.Split(' ');
                    int i = 0;
                    foreach (string item in paraList)
                    {
                        //nếu chứa '@' => nó là parameter
                        if (item.Contains('@'))
                        {
                            //Xoá dấu "," sau đít tham số
                            string para = item.Replace(",", "");
                            //thêm vào danh sách tên parameter và giá trị tương ứng được truyền cho nó, nếu nó null thì truyền vào DBNull.Value
                            command.Parameters.AddWithValue(para, parameter[i] ?? DBNull.Value);
                            //chỉ tăng i mỗi khi tìm được tên 1 parameter
                            i++;
                        }
                    }

                }

                data = command.ExecuteNonQuery();

            }

            return data;
        }

        /// <summary>
        /// Trả về cột đầu tiên của dòng đầu tiên trong bảng kết quả.
        /// </summary>
        /// <param name="query">Câu truy vấn.</param>
        /// <param name="parameter">Mảng các parameter.</param>
        /// <returns></returns>
        public object ExecuteScalar(string query, object[] parameter = null)
        {
            //tạo biến chứa kết quả trả về
            object data = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

                if (parameter != null)
                {
                    //cắt nhỏ câu truy vấn để lấy ra phần parameter dạng @abcxyz
                    string[] paraList = query.Split(' ');
                    int i = 0;
                    foreach (string item in paraList)
                    {
                        //nếu chứa '@' => nó là parameter
                        if (item.Contains('@'))
                        {
                            //Xoá dấu "," sau đít tham số
                            string para = item.Replace(",", "");
                            //thêm vào danh sách tên parameter và giá trị tương ứng được truyền cho nó, nếu nó null thì truyền vào DBNull.Value
                            command.Parameters.AddWithValue(para, parameter[i] ?? DBNull.Value);
                            //chỉ tăng i mỗi khi tìm được tên 1 parameter
                            i++;
                        }
                    }

                }

                data = command.ExecuteScalar();

            }

            return data;
        }

    }
}
