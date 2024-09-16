using WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserWishlistRepository
{
    Task<UserWishlist> GetUserWishlistAsync(int userId); // Método actualizado
    Task AddProductToWishlistAsync(int userId, Product product); // Método actualizado
    Task RemoveProductFromWishlistAsync(int userId, int productId); // Método actualizado
}
