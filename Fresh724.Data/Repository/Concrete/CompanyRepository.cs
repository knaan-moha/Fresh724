using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class CompanyRepository:EntityRepository<Company>, ICompanyRepository

{
    private ApplicationDbContext _db;
    public CompanyRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}