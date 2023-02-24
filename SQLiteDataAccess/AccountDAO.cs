using Interfaces;
using Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace SQLiteDataAccess
{
    public class AccountDAO : IAccountDAO
    {
        private AccountDAO() { }

        public bool DeleteAccount(string username)
        {
            string query = $"DELETE FROM Account WHERE UserName = @username ";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { username });

            if (result == 1) return true;

            return false;
        }

        public List<Account> GetListAccount()
        {
            string query = "SELECT Username, DisplayName, Type FROM Account";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            List<Account> listAccount = new List<Account>();

            foreach (DataRow row in data.Rows)
            {
                Account account = new Account(row);

                listAccount.Add(account);
            }

            return listAccount;
        }

        public Account InsertAccount(string username)
        {
            string query = $"SELECT COUNT(*) FROM Account WHERE Username = @username ";

            var result = Convert.ToInt32(DataProvider.Instance.ExecuteScalar(query, new object[] { username }));

            if (result > 0) return null;

            query = $"INSERT INTO Account(Username) VALUES( @username )";

            result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { username });

            if (result == 1)
            {
                query = "SELECT Username, DisplayName, Type FROM Account WHERE Username = @username ;";

                DataTable data = DataProvider.Instance.ExecuteQuery(query, new object[] { username });

                if (data.Rows.Count > 0)
                {
                    Account account = new Account(data.Rows[0]);
                    return account;
                }
                return null;
            }

            return null;
        }

        public Account Login(string username, string password)
        {
            string query = "SELECT Username, DisplayName, Type FROM Account WHERE Username = @username AND Password = @password ";
           
            byte[] bytePassword = ASCIIEncoding.ASCII.GetBytes(password);

            byte[] sha256Password = SHA256.Create().ComputeHash(bytePassword);

            string hashString = "";
            foreach (var item in sha256Password)
            {
                hashString += Convert.ToString(item);
            }

            DataTable data = DataProvider.Instance.ExecuteQuery(query, new object[] { username, hashString });

            if (data.Rows.Count == 1)
            {
                return new Account(data.Rows[0]);
            }

            return null;
        }

        /// <summary>
        /// Cập nhật Account.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="displayname"></param>
        /// <param name="password"></param>
        /// <param name="type"></param>
        /// <returns>
        /// Item1 - Kết quả cập nhật DisplayName.
        /// <br>Item2 - Kết quả cập nhật Password.</br>
        /// <br>Item3 - Kết quả cập nhật AccType.</br>
        /// </returns>
        public (bool, bool, bool) Update(string username, string displayname = null, string password = null, AccountType? type = null)
        {

            var result1 = 0;
            if (!string.IsNullOrEmpty(displayname))
            {
                string query = $"UPDATE Account SET DisplayName = @displayname WHERE Username = @username ;";
                result1 = DataProvider.Instance.ExecuteNonQuery(query, new object[] { displayname, username });
            }

            var result2 = 0;
            if (!string.IsNullOrEmpty(password))
            {
                byte[] bytePassword = Encoding.ASCII.GetBytes(password);

                byte[] sha256Password = SHA256.Create().ComputeHash(bytePassword);

                password = "";
                foreach (var item in sha256Password)
                {
                    password += Convert.ToString(item);
                }

                string query = @$"UPDATE Account SET Password = @password WHERE Username = @username ;";
                result2 = DataProvider.Instance.ExecuteNonQuery(query, new object[] { password, username });
            }

            var result3 = 0;
            if (type != null)
            {
                string query = $@"UPDATE Account SET Type = @type WHERE Username = @username ";
                result3 = DataProvider.Instance.ExecuteNonQuery(query, new object[] { (int)type, username });
            }

            bool item1 = false, item2 = false, item3 = false;
            if (result1 == 1) item1 = true;
            if (result2 == 1) item2 = true;
            if (result3 == 1) item3 = true;

            return (item1, item2, item3);
        }
    }
}
