using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using IUnitOfWork = Fresh724.Data.Context.ApplicationDbContext;

namespace Fresh724.Data.Repository.Concrete;

public class CategoryRepository: EntityRepository<Category>, ICategoryRepository
{
    private ApplicationDbContext _db;
    
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}