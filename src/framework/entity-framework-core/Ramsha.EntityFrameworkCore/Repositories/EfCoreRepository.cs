
using System.Linq.Expressions;
using System.Numerics;
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
    protected ISpecificationEvaluator SpecificationEvaluator { get; set; } = EntityFrameworkCore.EfCoreSpecificationEvaluator.Default;

    protected IDbContextProvider<TDbContext> DbContextProvider => ServiceProvider.GetLazyRequiredService<IDbContextProvider<TDbContext>>().Value;
    protected IUnitOfWorkManager UnitOfWorkManager => ServiceProvider.GetLazyRequiredService<IUnitOfWorkManager>().Value;


    public EFCoreRepository(ISpecificationEvaluator specificationEvaluator)
    {
        SpecificationEvaluator = specificationEvaluator;
    }
    public EFCoreRepository()
    {
    }
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
                query = query.Include(includes);
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

    public virtual async Task<TEntity?> AddAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
   {
       var context = await GetDbContextAsync();
       var entry = await context.Set<TEntity>().AddAsync(entity, cancellationToken);

       if (autoSave)
       {
           await context.SaveChangesAsync(cancellationToken);
       }
       return entry.Entity;
   });


    }

    public async Task<List<TEntity>> GetListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
    {
        var context = await GetDbContextAsync();
        var query = context.Set<TEntity>()
        .AsQueryable()
        .Include(includes);

        return await query.ToListAsync(cancellationToken);
    });

    }

    public async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
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
       {
           query = query.Include(includes);
       }

       return await query.Where(criteria).ToListAsync(cancellationToken);
   });

    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().CountAsync(cancellationToken);
});
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    return await context.Set<TEntity>().Where(criteria).CountAsync(cancellationToken);
});
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await UnitOfWork(async () =>
 {
     var context = await GetDbContextAsync();
     context.RemoveRange(entities);
     if (autoSave)
     {
         await context.SaveChangesAsync(cancellationToken);
     }
 });
    }

    public async Task<IQueryable<TEntity>> GetQueryAsync()
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return context.Set<TEntity>().AsQueryable();
        });
    }



    public async Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams paginationParams, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {

        var query = await GetQueryAsync();

        var total = await query.CountAsync(cancellationToken);
        var pagedResult = await query
             .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
             .Take(paginationParams.PageSize)
             .Include(includes)
             .AsNoTracking()
             .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));

    }

    public async Task<PagedResult<T>> GetPagedAsync<T>(PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();

        var total = await query.CountAsync(cancellationToken);
        var pagedResult = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Include(includes)
            .Select(mapping)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));

    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams paginationParams, Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {

        var query = await GetQueryAsync();

        var total = await query.CountAsync(criteria, cancellationToken);
        var pagedResult = await query
            .Where(criteria)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Include(includes)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));

    }

    public async Task<PagedResult<T>> GetPagedAsync<T>(PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();

        var total = await query.CountAsync(criteria, cancellationToken);
        var pagedResult = await query
            .Where(criteria)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Include(includes)
            .Select(mapping)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, paginationParams.PageSize, paginationParams.PageNumber));
    }


    public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            if (autoSave)
            {
                await context.SaveChangesAsync(cancellationToken);
            }
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
            var query = context.Set<TEntity>()
            .AsQueryable()
            .Include(includes);

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
            var query = context.Set<TEntity>()
            .AsQueryable()
            .Include(includes)
            .AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        });
    }

    public async Task<TEntity?> ReadOnlyAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>()
           .AsQueryable()
           .Include(includes);
            return await query.AsNoTracking()
            .FirstOrDefaultAsync(criteria, cancellationToken);
        });
    }

    public async Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification)
               .ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetListAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return GetListAsync(spec, cancellationToken);
    }

    public async Task<List<T>> GetListAsync<T>(ISpecification<TEntity, T> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification)
               .ToListAsync(cancellationToken);
    }

    public Task<List<T>> GetListAsync<T>(Action<ISpecificationBuilder<TEntity, T>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity, T>(specBuilder);
        return GetListAsync(spec, cancellationToken);
    }

    protected virtual IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false)
    {
        return SpecificationEvaluator.GetQuery(query, specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(IQueryable<TEntity> query, ISpecification<TEntity, TResult> specification)
    {
        return SpecificationEvaluator.GetQuery(query, specification);
    }

    public async Task<int> CountAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return await CountAsync(spec, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification, true)
               .CountAsync(cancellationToken);
    }

    public async Task<TEntity?> FindAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return await FindAsync(spec, cancellationToken);
    }

    public async Task<TEntity?> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification)
               .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return await ReadOnlyListAsync(spec, cancellationToken);
    }

    public async Task<TEntity?> ReadOnlyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification)
               .AsNoTracking()
               .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> ReadOnlyAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return await ReadOnlyAsync(spec, cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync<T>(IPageSpecification<TEntity, T> pageSpecification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();

        var total = await CountAsync(pageSpecification, cancellationToken);
        var pagedResult = await
        ApplySpecification(query, pageSpecification)
            .ToListAsync(cancellationToken);

        return RamshaResults.Paged(pagedResult, new RamshaPagedInfo(total, pageSpecification.PaginationParams.PageSize, pageSpecification.PaginationParams.PageNumber));
    }

    public async Task<IReadOnlyList<T>> ReadOnlyListAsync<T>(ISpecification<TEntity, T> specification, CancellationToken cancellationToken = default)
    where T : class
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ReadOnlyListAsync<T>(Action<ISpecificationBuilder<TEntity, T>> specBuilder, CancellationToken cancellationToken = default)
      where T : class
    {
        var spec = new Specification<TEntity, T>(specBuilder);
        return await ReadOnlyListAsync(spec, cancellationToken);
    }




    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return await context.Set<TEntity>().AnyAsync(cancellationToken);
        });
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return await context.Set<TEntity>().AnyAsync(criteria, cancellationToken);
        });
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification, true)
            .AnyAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return await AnyAsync(spec, cancellationToken);
    }


    public async Task<bool> AllAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            return await context.Set<TEntity>().AllAsync(criteria, cancellationToken);
        });
    }

    public async Task<bool> AllAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        return await ApplySpecification(query, specification, true)
            .AllAsync(_ => true, cancellationToken);
    }

    public async Task<bool> AllAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default)
    {
        var spec = new Specification<TEntity>(specBuilder);
        return await AllAsync(spec, cancellationToken);
    }


    public async Task<TResult?> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.MaxAsync(selector, cancellationToken);
        });
    }

    public async Task<TResult?> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.MaxAsync(selector, cancellationToken);
        });
    }

    public async Task<TResult?> MaxAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryAsync();
        var projected = ApplySpecification(query, specification);
        return await projected.MaxAsync(cancellationToken);

    }


    public async Task<TEntity?> MaxByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return query.MaxBy(keySelector);

        });
    }

    public async Task<TEntity?> MaxByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>>? criteria = null, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>()
            .AsQueryable()
            .Include(includes);

            if (criteria is not null)
            {
                query = query.Where(criteria);
            }

            return query.MaxBy(keySelector);

        });
    }




    public async Task<TResult?> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.MinAsync(selector, cancellationToken);

        });
    }

    public async Task<TResult?> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.MinAsync(selector, cancellationToken);

        });
    }




    public async Task<TEntity?> MinByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return query.MinBy(keySelector);

        });
    }

    public async Task<TEntity?> MinByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>>? criteria = null, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable().Include(includes);

            if (criteria is not null)
            {
                query = query.Where(criteria);
            }

            return query.MinBy(keySelector);
        });
    }




    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)

    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            return await query.SumAsync(selector, cancellationToken);
        });
    }

    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.SumAsync(selector, cancellationToken);

        });
    }

    public async Task<double> SumAsync(Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default)

    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            return await query.SumAsync(selector, cancellationToken);
        });
    }

    public async Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.SumAsync(selector, cancellationToken);

        });
    }

    public async Task<float> SumAsync(Expression<Func<TEntity, float>> selector, CancellationToken cancellationToken = default)

    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            return await query.SumAsync(selector, cancellationToken);
        });
    }

    public async Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.SumAsync(selector, cancellationToken);

        });
    }

    public async Task<long> SumAsync(Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default)

    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            return await query.SumAsync(selector, cancellationToken);
        });
    }

    public async Task<long> SumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.SumAsync(selector, cancellationToken);

        });
    }

    public async Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)

    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();

            return await query.SumAsync(selector, cancellationToken);
        });
    }

    public async Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.SumAsync(selector, cancellationToken);

        });
    }


    public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<float> AverageAsync(Expression<Func<TEntity, float>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<float> AverageAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<double> AverageAsync(Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<double> AverageAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<double> AverageAsync(Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<double> AverageAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().AsQueryable();
            return await query.AverageAsync(selector, cancellationToken);
        });
    }

    public async Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
        {
            var context = await GetDbContextAsync();
            var query = context.Set<TEntity>().Where(criteria);
            return await query.AverageAsync(selector, cancellationToken);
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
    var query = context.Set<TEntity>()
    .AsQueryable()
    .Include(includes);

    return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
});
    }

    public async Task<bool> DeleteAsync(TId id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork(async () =>
{
    var context = await GetDbContextAsync();
    var entity = await context.Set<TEntity>().FindAsync(id);
    if (entity is null)
    {
        return false;
    }
    if (autoSave)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    context.Remove(entity);
    return true;
});
    }

    public async Task<bool> AnyAsync(TId id, CancellationToken cancellationToken = default)
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
            var query = context.Set<TEntity>()
            .AsQueryable()
            .Include(includes);
            return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        });
    }
}