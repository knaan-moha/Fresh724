using System.Linq.Expressions;
using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PagedList;
using IUnitOfWork = Fresh724.Data.Context.ApplicationDbContext;

namespace Fresh724.Data.Repository.Concrete;


    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<TEntity> dbSet;

        public EntityRepository(ApplicationDbContext db)
        {
            _db= db;
            //_db.ShoppingCarts.Include(u => u.Product).Include(u=>u.CoverType);
            this.dbSet= _db.Set<TEntity>();
        }

        public List<TEntity> GetByFilter(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList();
        }

        public List<TEntity> OrderBy(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.OrderBy(filter);
            }
            return query.ToList();
        }
        
        public List<TEntity> OrderByDescending(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.OrderByDescending(filter);
            }
            return query.ToList();
        }
        

            public void Add(TEntity entity)
        {
            dbSet.Add(entity);
           
        }
        //includeProp - "Category,CoverType"
        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter=null, string? includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach(var includeProp in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            if (tracked)
            {
                IQueryable<TEntity> query = dbSet;

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }
            else
            {
                IQueryable<TEntity> query = dbSet.AsNoTracking();

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }
            
        }
        
        
        public async Task<IEnumerable<TEntity>> GetManyAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? top  = null,
            int? skip = null,
            params string[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;
 
            if (filter != null)
            {
                query = query.Where(filter);
            }
 
            if (includeProperties.Length > 0)
            {
                query = includeProperties.Aggregate(query, (theQuery, theInclude) => theQuery.Include(theInclude));
            }
 
            if (orderBy != null)
            {
                query = orderBy(query);
            }
 
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
 
            if (top.HasValue)
            {
                query = query.Take(top.Value);
            }
 
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }
        
        public Task DeleteAsync(TEntity entity)
        {
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        
        public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entities = dbSet.Where(filter);
 
            dbSet.RemoveRange(entities);
 
            return Task.CompletedTask;
        }
        
        public void ToList()
        {
            dbSet.ToList();
        }
        
       /* public IEnumerable<Category> ToPagedList()
        {
            dbSet.ToList().ToPagedList(1, 2);
            return dbSet.ToList();
        }*/

        
        public void Update(TEntity entity)
        {
            dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entity)
        {
            dbSet.RemoveRange(entity);
        }
        
       
        
       
    }


