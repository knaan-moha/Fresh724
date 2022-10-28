using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Concrete;

public class EmployeeRepository:EntityRepository<Employee>, IEmployeeRepository
{
    private ApplicationDbContext _db;
    
    public EmployeeRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }
}