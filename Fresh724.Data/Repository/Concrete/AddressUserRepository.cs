using System.Linq.Expressions;
using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class AddressUserRepository: EntityRepository<AddressUser>, IAddressUserRepository
{
    private ApplicationDbContext _db;
    
    public AddressUserRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }
    
}