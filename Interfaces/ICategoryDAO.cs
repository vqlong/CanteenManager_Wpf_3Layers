using Models;

namespace Interfaces
{
    public interface ICategoryDAO
    {
        Category GetCategoryById(int categoryId);
        List<Category> GetListCategory();
        List<Category> GetListCategoryServing();
        Category InsertCategory(string name);
        bool UpdateCategory(int id, string name, UsingState categoryStatus);
        bool DeleteCategory(int id);
    }
}