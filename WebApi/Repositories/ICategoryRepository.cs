using WebApi.Models;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> GetCategoryByNameAsync(string name);
    Task<Category> AddCategoryAsync(Category category);
}