using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BUS
{
    public class FoodBUS
    {
        private FoodBUS()
        {

        }

        private static readonly FoodBUS instance = new FoodBUS();
        public static FoodBUS Instance => instance;
        IFoodDAO Food => Config.Container.Resolve<IFoodDAO>();

        public Food GetFoodById(int foodId) => Food.GetFoodById(foodId);
        public List<Food> GetListFood() => Food.GetListFood();
        public List<Food> GetListFoodByCategoryId(int categoryId) => GetListFoodByCategoryId(categoryId);
        public object GetRevenueByFoodAndDate(DateTime fromDate, DateTime toDate) => Food.GetRevenueByFoodAndDate(fromDate, toDate);
        public Food InsertFood(string name, int categoryId, double price) => Food.InsertFood(name, categoryId, price);
        public List<Food> SearchFood(string input, bool option) => Food.SearchFood(input, option);
        public bool UpdateFood(int id, string name, int categoryId, double price, UsingState foodStatus) => Food.UpdateFood(id, name, categoryId, price, foodStatus);
        public bool DeleteFood(int Id) => Food.DeleteFood(Id);
    }
}
