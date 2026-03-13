using System.Linq.Expressions;

namespace Ramsha.Common.Domain;

public interface IRepository
{

}

public interface IRepository<TEntity> : IRepository
where TEntity : IEntity
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default);
    Task<TEntity?> ReadOnlyAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<IQueryable<TEntity>> GetQueryableAsync();

    Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync<T>(PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> GetPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync<T>(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction, PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, CancellationToken cancellationToken = default);

}

public interface IRepository<TEntity, TId> : IRepository<TEntity>
where TId : IEquatable<TId>
where TEntity : IEntity<TId>
{
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> IsExist(TId id, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(TId id, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<TEntity?> ReadOnlyAsync(TId id, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);


}




