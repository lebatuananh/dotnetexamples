using System;
using System.Linq.Expressions;

namespace Shared.Specification;

public class NoOpSpec<TEntity> : SpecificationBase<TEntity>
{
    public override Expression<Func<TEntity, bool>> Criteria => p => true;
}