using WebApi.Models;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
    Task<Product> AddProductAsync(Product product);
    Task DeleteProductAsync(int id);
}