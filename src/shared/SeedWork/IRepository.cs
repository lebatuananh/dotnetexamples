using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shared.SeedWork;

public interface IRepository<T> where T : IAggregateRoot
{
    Task CommitAsync();
    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

    Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

    IQueryable<T> FindAll(params Expression<Func<T, object>>[] includeProperties);

    IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);

    void Delete(Guid id);
    void DeleteMany(List<T> entities);
}