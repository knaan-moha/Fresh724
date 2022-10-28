using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class OrderShoppingRepository:EntityRepository<OrderShopping>,IOrderShoppingRepository
{
    private ApplicationDbContext _db;
    public OrderShoppingRepository(ApplicationDbContext context) : base(context)
    {
        _db = context;
    }
}
