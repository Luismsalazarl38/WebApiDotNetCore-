using System.Collections.Generic;

namespace WebApi.Models
{
    public class UserWishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
