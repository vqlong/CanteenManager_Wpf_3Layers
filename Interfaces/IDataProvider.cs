using System.Data;

namespace Interfaces
{
    public interface IDataProvider
    {
        string Name { get; }
        int ExecuteNonQuery(string query, object[] parameter = null);
        DataTable ExecuteQuery(string query, object[] parameter = null);
        object ExecuteScalar(string query, object[] parameter = null);
        bool TestConnection(string connectionString);
    }
}