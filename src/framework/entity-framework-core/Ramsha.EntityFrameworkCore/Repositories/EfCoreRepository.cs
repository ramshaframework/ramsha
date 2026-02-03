
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

    public virtual async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(expression);
        });

    }

    public virtual async Task<List<TEntity>> GetListAsync()
    {
        return await UnitOfWork(async () =>
      {
          var context = await GetDbContextAsync();
          return await context.Set<TEntity>().ToListAsync();
      });
    }

    public virtual async Task<TEntity?> AddAsync(TEntity entity)
    {
        return await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       var entry = await context.Set<TEntity>().AddAsync(entity);
       return entry.Entity;
   });


    }

    public async Task<List<TEntity>> GetListAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        return await UnitOfWork(async () =>
    {
        var context = await GetDbContextAsync();
        var query = context.Set<TEntity>().AsQueryable();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync();
    });

    }

    public async Task DeleteAsync(TEntity entity)
    {
        await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       context.Remove(entity);
   });
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> criteria, params Expression<Func<TEntity, object>>[] includes)
    {
        return await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       var query = context.Set<TEntity>().AsQueryable();
       foreach (var include in includes)
       {
           query = query.Include(include);
       }
       return await query.Where(criteria).ToListAsync();
   });

    }

    public async Task<int> GetCountAsync()
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().CountAsync();
});
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> criteria)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().Where(criteria).CountAsync();
});
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        await UnitOfWork(async () =>
 {
     var context = await GetDbContextAsync();
     context.RemoveRange(entities);
 });
    }

    public async Task<IQueryable<TEntity>> QueryAsync()
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return  context.Set<TEntity>().AsQueryable();
        });
    }
}


public class EFCoreRepository<TDbContext, TEntity, TId> : EFCoreRepository<TDbContext, TEntity>, IRepository<TEntity, TId>
where TDbContext : IEFDbContext
where TId : IEquatable<TId>
where TEntity : class, IEntity<TId>

{
    public async Task<TEntity?> FindAsync(TId id)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().FindAsync(id);
});
    }

    public async Task<TEntity?> FindAsync(TId id, params Expression<Func<TEntity, object>>[] includes)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    var query = context.Set<TEntity>().AsQueryable();
    foreach (var include in includes)
    {
        query = query.Include(include);
    }
    return await query.FirstOrDefaultAsync(x => x.Id.Equals(id));
});
    }

    public async Task<bool> DeleteAsync(TId id)
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

    public async Task<bool> IsExist(TId id)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    var existEntity = await context.Set<TEntity>().FindAsync(id);
    return existEntity is not null;
});
    }
}