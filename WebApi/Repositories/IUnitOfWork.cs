public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IUserWishlistRepository UserWishlists { get; }
    Task<int> CompleteAsync();
}