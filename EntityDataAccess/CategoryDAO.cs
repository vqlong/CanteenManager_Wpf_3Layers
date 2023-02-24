using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class CategoryDAO : Interfaces.ICategoryDAO
    {
        private CategoryDAO() { }
        private static readonly CategoryDAO instance = new CategoryDAO();
        public static CategoryDAO Instance => instance;

        public Category GetCategoryById(int categoryId)
        {
            using var context = new CanteenContext();

            return context.Categories.Include(c => c.Foods).FirstOrDefault(c => c.Id == categoryId);
        }

        public List<Category> GetListCategory()
        {
            using var context = new CanteenContext();

            return context.Categories.Include(c => c.Foods).ToList();
        }

        public List<Category> GetListCategoryServing()
        {
            using var context = new CanteenContext();

            return context.Categories.Where(c => c.CategoryStatus == UsingState.Serving).Include(c => c.Foods.Where(f => f.FoodStatus == UsingState.Serving)).ToList();
        }

        public Category InsertCategory(string name)
        {
            using var context = new CanteenContext();
            var category = new Category { Name = name };
            context.Categories.Add(category);
            if (context.SaveChanges() == 1) return category;
            return null;
        }

        public bool UpdateCategory(int id, string name, UsingState categoryStatus)
        {
            using var context = new CanteenContext();
            context.Categories.Update(new Category { Id = id, Name = name, CategoryStatus = categoryStatus });
            //var foods = FoodDAO.Instance.GetListFoodByCategoryId(id);
            //foreach (var food in foods)
            //{
            //    food.FoodStatus = categoryStatus;
            //    context.Foods.Update(food);
            //}
            context.Foods.Where(f => f.CategoryId == id).ExecuteUpdate(spc => spc.SetProperty(f => f.FoodStatus, categoryStatus));
            if (context.SaveChanges() == 1) return true;
            return false;
        }

        public bool DeleteCategory(int id)
        {
            using var context = new CanteenContext();

            context.Categories.Attach(new Category { Id = id, CategoryStatus = UsingState.StopServing }).Property(c => c.CategoryStatus).IsModified = true;
            //var foods = FoodDAO.Instance.GetListFoodByCategoryId(id);
            //foreach (var food in foods)
            //{
            //    food.FoodStatus = UsingState.StopServing;
            //    context.Foods.Update(food);
            //}
            context.Foods.Where(f => f.CategoryId == id).ExecuteUpdate(spc => spc.SetProperty(f => f.FoodStatus, UsingState.StopServing));

            if (context.SaveChanges() == 1) return true;
            return false;
        }
    }
}
