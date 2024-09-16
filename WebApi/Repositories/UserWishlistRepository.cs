using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class UserWishlistRepository : IUserWishlistRepository
    {
        private readonly AppDbContext _context;

        public UserWishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserWishlist> GetUserWishlistAsync(int userId)
        {
            return await _context.UserWishlists
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task AddProductToWishlistAsync(int userId, Product product)
        {
            var wishlist = await _context.UserWishlists
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist != null)
            {
                wishlist.Products.Add(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveProductFromWishlistAsync(int userId, int productId)
        {
            var wishlist = await _context.UserWishlists
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist != null)
            {
                var product = wishlist.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    wishlist.Products.Remove(product);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
