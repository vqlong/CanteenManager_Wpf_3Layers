using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class FoodDAO : IFoodDAO
    {
        private FoodDAO() { }
        private static readonly FoodDAO instance = new FoodDAO();
        public static FoodDAO Instance => instance;

        public List<Food> GetListFood()
        {
            using var context = new CanteenContext();

            return context.Foods.OrderBy(f => f.CategoryId).Include(f => f.Category).ToList();  
        }

        public List<Food> GetListFoodByCategoryId(int categoryId)
        {
            using var context = new CanteenContext();

            return context.Foods.Where(f => f.CategoryId == categoryId).ToList();
        }

        public object GetRevenueByFoodAndDate(DateTime fromDate, DateTime toDate)
        {
            using var context = new CanteenContext();

            var data = context.BillInfos.Where(bi => bi.Bill.DateCheckIn >= fromDate && bi.Bill.DateCheckOut <= toDate && bi.Bill.Status == 1)
                                        .Select(bi => new {
                                            Name = bi.Food.Name, 
                                            FoodCount = bi.FoodCount, 
                                            TotalPrice = bi.FoodCount * bi.Food.Price
                                        })
                                        .GroupBy(e => e.Name)
                                        .Select(g => new { 
                                            Name = g.Key, 
                                            TotalFoodCount = g.Sum(e => e.FoodCount), 
                                            Revenue = g.Sum(e => e.TotalPrice)
                                        })
                                        .ToList();

            return data;
        }

        public Food InsertFood(string name, int categoryId, double price)
        {
            using var context = new CanteenContext();
            var food = new Food { Name = name, CategoryId = categoryId, Price = price };
            context.Foods.Add(food);
            if (context.SaveChanges() == 1)
            {
                food.Category = CategoryDAO.Instance.GetCategoryById(categoryId);
                return food;
            }
            return null;
        }

        public List<Food> SearchFood(string input, bool option)
        {
            using var cxt = new CanteenContext();

            if (option) return cxt.Foods.Where(f => cxt.ToUnsigned(f.Name).Contains(cxt.ToUnsigned(input)))
                                        .OrderBy(f => f.CategoryId)
                                        .ToList();

            return cxt.Foods.Where(f => f.Name.Contains(input))
                            .OrderBy(f => f.CategoryId)
                            .ToList();
        }

        public bool UpdateFood(int id, string name, int categoryId, double price, UsingState foodStatus)
        {
            var category = CategoryDAO.Instance.GetCategoryById(categoryId);
            if (category.CategoryStatus == UsingState.StopServing) return false;

            using var context = new CanteenContext();
            context.Foods.Update(new Food { Id = id, Name = name, CategoryId = categoryId, Price = price, FoodStatus = foodStatus });
            if (context.SaveChanges() == 1) return true;
            return false;
        }

        public Food GetFoodById(int Id)
        {
            using var context = new CanteenContext();

            return context.Foods.Include(f => f.Category).FirstOrDefault(f => f.Id == Id);
        }

        public bool DeleteFood(int id)
        {
            using var context = new CanteenContext();

            context.Foods.Attach(new Food { Id = id, FoodStatus = UsingState.StopServing }).Property(f => f.FoodStatus).IsModified = true;
            if (context.SaveChanges() == 1) return true;
            return false;
        }

    }
}
