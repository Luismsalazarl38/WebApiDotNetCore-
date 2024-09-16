
using WebApi.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IProductRepository Products { get; private set; }
    public ICategoryRepository Categories { get; private set; }
    public IUserWishlistRepository UserWishlists { get; private set; }

    public UnitOfWork(AppDbContext context, IProductRepository products, ICategoryRepository categories, IUserWishlistRepository userWishlists)
    {
        _context = context;
        Products = products;
        Categories = categories;
        UserWishlists = userWishlists;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
}