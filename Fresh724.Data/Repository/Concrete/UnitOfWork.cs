using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using IUnitOfWork = Fresh724.Data.Context.ApplicationDbContext;

namespace Fresh724.Data.Repository.Concrete;

public class UnitOfWork: Abstract.IUnitOfWork
{
    private ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Categories = new CategoryRepository(_db);
        Employees  = new EmployeeRepository(_db);
        Products = new ProductRepository(_db);
        Companies = new CompanyRepository(_db);
        ApplicationUsers = new ApplicationUserRepository(_db);
        ShoppingCarts = new ShoppingCartRepository(_db);
        AddressUsers = new AddressUserRepository(_db);
        Orders = new OrderShoppingRepository(_db);

    }

    public IApplicationUserRepository ApplicationUsers { get; private set;}
    public ICategoryRepository Categories { get; private set;}
    public IProductRepository Products { get;  private set; }
    public IOrderShoppingRepository Orders { get; private set; }
    public IEmployeeRepository Employees { get; private set; }
    public ICompanyRepository Companies { get; private set;}
    public IShoppingCartRepository ShoppingCarts { get; private set; }
    public AddressUserRepository AddressUsers { get; private set; }

    public void SaveChanges()
    {
        _db.SaveChangesAsync();
    }
}

