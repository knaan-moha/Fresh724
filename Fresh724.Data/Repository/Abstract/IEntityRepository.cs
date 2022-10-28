using System.Linq.Expressions;
using Fresh724.Entity.Entities;

namespace Fresh724.Data.Repository.Abstract;

public interface IEntityRepository<TEntity> where TEntity : class
{
 
    TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter, string? includeProperties = null,bool tracked = true);
    IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter=null, string? includeProperties = null);
    public List<TEntity> GetByFilter(Expression<Func<TEntity, bool>> filter = null);
    void Add(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entity);
    void Update(TEntity entity);

    public void ToList();

  //  public IEnumerable<Category> ToPagedList();

  public List<TEntity> OrderByDescending(Expression<Func<TEntity, bool>> filter = null);
  public List<TEntity> OrderBy(Expression<Func<TEntity, bool>> filter = null);

    public Task<IEnumerable<TEntity>> GetManyAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int? top = null,
        int? skip = null,
        params string[] includeProperties);
    
    public Task<TEntity> GetByIdAsync(Guid id);
    public Task<TEntity> AddAsync(TEntity entity);
    
    public Task DeleteAsync(TEntity entity) ;
    
    public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filter) ;


}