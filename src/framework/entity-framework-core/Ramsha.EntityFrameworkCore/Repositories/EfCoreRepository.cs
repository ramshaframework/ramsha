
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Common.Domain;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.EntityFrameworkCore;

public class EFCoreRepository<TDbContext, TEntity> : IRepository<TEntity>
where TDbContext : IEFDbContext
where TEntity : class, IEntity
{
    [Injectable]
    public IServiceProvider ServiceProvider { get; set; } = default!;
    protected IDbContextProvider<TDbContext> DbContextProvider => ServiceProvider.GetLazyRequiredService<IDbContextProvider<TDbContext>>().Value;
    protected IUnitOfWorkManager UnitOfWorkManager => ServiceProvider.GetLazyRequiredService<IUnitOfWorkManager>().Value;
    protected Task<T> TransactionalUnitOfWork<T>(Func<Task<T>> action)
    {
        return UnitOfWork(action, true);
    }
    protected Task TransactionalUnitOfWork(Func<Task> action)
    {
        return UnitOfWork(action, true);
    }

    protected async Task<T> UnitOfWork<T>(
     Func<Task<T>> action, bool isTransactional = false)
    {
        if (UnitOfWorkManager.Current is not null)
        {
            return await action();
        }
        var options = new UnitOfWorkOptions();
        options.IsTransactional = isTransactional;

        if (UnitOfWorkManager.TryBeginReserved(
                RamshaUnitOfWorkReservationNames.ActionUnitOfWorkReservationName,
                options))
        {
            var result = await action();
            if (UnitOfWorkManager.Current is not null)
            {
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            return result;
        }

        using (var uow = UnitOfWorkManager.Begin(options))
        {
            var result = await action();
            await uow.CompleteAsync();
            return result;
        }
    }


    protected async Task UnitOfWork(Func<Task> action, bool isTransactional = false)
    {
        if (UnitOfWorkManager.Current is not null)
        {
            await action();
            return;
        }
        var options = new UnitOfWorkOptions();
        options.IsTransactional = isTransactional;

        if (UnitOfWorkManager.TryBeginReserved(
                RamshaUnitOfWorkReservationNames.ActionUnitOfWorkReservationName,
                options))
        {
            await action();
            if (UnitOfWorkManager.Current is not null)
            {
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            return;
        }

        using (var uow = UnitOfWorkManager.Begin(options))
        {
            await action();
            await uow.CompleteAsync();
        }
    }
    protected virtual async Task<TDbContext> GetDbContextAsync()
    => await DbContextProvider.GetDbContextAsync();

    public virtual async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> expression, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(expression, cancellationToken);
        });

    }

    public virtual async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
      {
          var context = await GetDbContextAsync();
          return await context.Set<TEntity>().ToListAsync(cancellationToken);
      });
    }

    public virtual async Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       var entry = await context.Set<TEntity>().AddAsync(entity, cancellationToken);
       return entry.Entity;
   });


    }

    public async Task<List<TEntity>> GetListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
    {
        var context = await GetDbContextAsync();
        var query = context.Set<TEntity>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync(cancellationToken);
    });

    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       context.Remove(entity);
   });
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       var query = context.Set<TEntity>().AsQueryable();

       if (includes is not null)
           foreach (var include in includes)
           {
               query = query.Include(include);
           }
       return await query.Where(criteria).ToListAsync(cancellationToken);
   });

    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().CountAsync(cancellationToken);
});
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().Where(criteria).CountAsync(cancellationToken);
});
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await UnitOfWork(async () =>
 {
     var context = await GetDbContextAsync();
     context.RemoveRange(entities);
 });
    }

    public async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return context.Set<TEntity>().AsQueryable();
        });
    }


    public async Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        var total = await query.CountAsync(cancellationToken);
        var pagedResult = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));
    }

    public async Task<PagedResult<T>> GetPagedAsync<T>(PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        var total = await query.CountAsync(cancellationToken);
        var pagedResult = await query
        .Select(mapping)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        queryAction(query);
        var total = await query.CountAsync(cancellationToken);
        var pagedResult = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));
    }

    public async Task<PagedResult<T>> GetPagedAsync<T>(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction, PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = queryAction(query);

        var total = await query.CountAsync(cancellationToken);
        var pagedResult = await query
        .Select(mapping)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        });
    }

    public async Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return await context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        });
    }

    public async Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            if (includes is not null)
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            return await query.Where(criteria)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        });
    }

    public async Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        });
    }

    public async Task<TEntity?> ReadOnlyAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.AsNoTracking()
            .FirstOrDefaultAsync(criteria, cancellationToken);
        });
    }
}


public class EFCoreRepository<TDbContext, TEntity, TId> : EFCoreRepository<TDbContext, TEntity>, IRepository<TEntity, TId>
where TDbContext : IEFDbContext
where TId : IEquatable<TId>
where TEntity : class, IEntity<TId>

{
    public async Task<TEntity?> FindAsync(TId id, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    var query = context.Set<TEntity>().AsQueryable();

    if (includes is not null)
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
    return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
});
    }

    public async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    var entity = await context.Set<TEntity>().FindAsync(id);
    if (entity is null)
    {
        return false;
    }

    context.Remove(entity);
    return true;
});
    }

    public async Task<bool> IsExist(TId id, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    var existEntity = await context.Set<TEntity>().FindAsync(id, cancellationToken);
    return existEntity is not null;
});
    }

    public async Task<TEntity?> ReadOnlyAsync(TId id, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            if (includes is not null)
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        });
    }
}