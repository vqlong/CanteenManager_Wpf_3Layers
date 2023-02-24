using Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class DataProvider : IDataProvider
    {
        private DataProvider() { }

        private static readonly DataProvider instance = new DataProvider();
        public static DataProvider Instance => instance;

        public string Name => "EFCore-SqlServer";

        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string query, object[] parameter = null)
        {
            throw new NotImplementedException();
        }

        public bool TestConnection(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT 1", connection);

                    if ((int)command.ExecuteScalar() == 1)
                    {
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
    }
}
