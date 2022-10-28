using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class ProductRepository : EntityRepository<Product>, IProductRepository
{
   private ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}