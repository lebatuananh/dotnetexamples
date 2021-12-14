using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.SeedWork;

namespace Shared.Repositories;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    protected readonly IMediator Mediator;
    protected readonly IScopeContext ScopeContext;

    protected BaseDbContext()
    {
    }

    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    protected BaseDbContext(
        DbContextOptions options,
        IMediator mediator,
        IScopeContext scopeContext) : base(options)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        ScopeContext = scopeContext ?? throw new ArgumentNullException(nameof(scopeContext));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTrack();
        var result = await base.SaveChangesAsync(cancellationToken);
        await Mediator.DispatchDomainEventsAsync(this);
        return result;
    }

    private void AddTrack()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
        foreach (var entry in entities)
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is ModifierTrackingEntity modifier)
                        modifier.MarkCreated(ScopeContext.CurrentAccountId, ScopeContext.CurrentAccountName);
                    if (entry.Entity is IDateTracking ent) ent.MarkCreated();

                    if (entry.Entity.GetType().GetCustomAttribute<PredefinedObjectAttribute>() != null)
                        entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Modified:
                    if (entry.Entity is ModifierTrackingEntity modifier2)
                        modifier2.MarkModified(ScopeContext.CurrentAccountId, ScopeContext.CurrentAccountName);
                    if (entry.Entity is IDateTracking ent2)
                    {
                        ent2.MarkModified();
                        entry.Property("CreatedDate").IsModified = false;
                        entry.Property("LastUpdatedDate").IsModified = true;
                    }

                    if (entry.Entity.GetType().GetCustomAttribute<PredefinedObjectAttribute>() != null)
                        entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Unchanged:
                case EntityState.Detached:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}