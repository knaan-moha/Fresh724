using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class ApplicationUserRepository:EntityRepository<ApplicationUser>, IApplicationUserRepository
{
private ApplicationDbContext _db;
    
public ApplicationUserRepository(ApplicationDbContext db) : base(db)
{
    _db = db;
}

    
}