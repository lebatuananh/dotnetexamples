using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace Shared.SeedWork;

public abstract class Repository<T, TDbContext> : IRepository<T>
    where T : Entity, IAggregateRoot where TDbContext : BaseDbContext
{
    public Repository(TDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
    }

    public IUnitOfWork Uow => _dbContext;

    protected TDbContext _dbContext { get; }
    protected DbSet<T> _dbSet => _dbContext.Set<T>();

    public async Task CommitAsync()
    {
        await Uow.SaveChangesAsync();
    }

    public IQueryable<T> FindAll(params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> items = _dbContext.Set<T>();
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);
        return items;
    }

    public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> items = _dbContext.Set<T>();
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);
        return items.Where(predicate);
    }

    public virtual async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).FirstOrDefaultAsync(predicate);
    }

    public void Delete(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Deleted;
    }

    public void Delete(Guid id)
    {
        var entity = _dbContext.Set<T>().Find(id);
        if (entity != null) _dbContext.Entry(entity).State = EntityState.Deleted;
    }

    public void DeleteMany(List<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }


    public void Update(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }
}