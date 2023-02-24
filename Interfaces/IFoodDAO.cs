using Models;
using System.Data;

namespace Interfaces
{
    public interface IFoodDAO
    {
        Food GetFoodById(int Id);
        List<Food> GetListFood();
        List<Food> GetListFoodByCategoryId(int categoryId);
        object GetRevenueByFoodAndDate(DateTime fromDate, DateTime toDate);
        Food InsertFood(string name, int categoryId, double price);
        List<Food> SearchFood(string input, bool option);
        bool UpdateFood(int id, string name, int categoryId, double price, UsingState foodStatus);
        bool DeleteFood(int id);
    }
}