using Models;

namespace Interfaces
{
    public interface ITableDAO
    {
        bool CombineTable(int firstTableId, int secondTableId);
        List<Table> GetListTable();
        List<Table> GetListTableUsing();
        Table GetTableById(int tableId);
        Table InsertTable();
        bool SwapTable(int firstTableId, int secondTableId);
        bool UpdateTable(int id, string name, UsingState usingState);
        bool DeleteTable(int id);
    }
}