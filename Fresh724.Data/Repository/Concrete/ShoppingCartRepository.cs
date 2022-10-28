using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class ShoppingCartRepository : EntityRepository<ShoppingCart>, IShoppingCartRepository
{
    private ApplicationDbContext _db;

    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
        
    }

}  
