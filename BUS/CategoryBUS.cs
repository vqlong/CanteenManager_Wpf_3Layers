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
    public class CategoryBUS
    {
        private CategoryBUS()
        {

        }

        private static readonly CategoryBUS instance = new CategoryBUS();
        public static CategoryBUS Instance => instance;
        ICategoryDAO Category => Config.Container.Resolve<ICategoryDAO>();

        public Category GetCategoryById(int categoryId) => Category.GetCategoryById(categoryId);
        public List<Category> GetListCategory() => Category.GetListCategory();
        public List<Category> GetListCategoryServing()=> Category.GetListCategoryServing();
        public Category InsertCategory(string name) => Category.InsertCategory(name);
        public bool UpdateCategory(int id, string name, UsingState categoryStatus) => Category.UpdateCategory(id, name, categoryStatus);
        public bool DeleteCategory(int id) => Category.DeleteCategory(id);
    }
}
