using System.Linq.Expressions;

namespace Ramsha.Common.Domain;

public interface IRepository
{

}

public interface IRepository<TEntity> : IRepository
where TEntity : IEntity
{


    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    Task<TEntity?> AddAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default);

    Task<List<T>> GetListAsync<T>(ISpecification<TEntity, T> specification, CancellationToken cancellationToken = default);
    Task<List<T>> GetListAsync<T>(Action<ISpecificationBuilder<TEntity, T>> specBuilder, CancellationToken cancellationToken = default);

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);


    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(IEnumerable<Expression<Func<TEntity, object>>> includes, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ReadOnlyListAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ReadOnlyListAsync<T>(ISpecification<TEntity, T> specification, CancellationToken cancellationToken = default) where T : class;
    Task<IReadOnlyList<T>> ReadOnlyListAsync<T>(Action<ISpecificationBuilder<TEntity, T>> specBuilder, CancellationToken cancellationToken = default) where T : class;

    Task<TEntity?> ReadOnlyAsync(Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<TEntity?> ReadOnlyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> ReadOnlyAsync(Action<ISpecificationBuilder<TEntity>> specBuilder, CancellationToken cancellationToken = default);

    Task<IQueryable<TEntity>> GetQueryAsync();

    Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams paginationParams, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync<T>(PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);

    Task<PagedResult<T>> GetPagedAsync<T>(IPageSpecification<TEntity, T> pageSpecification, CancellationToken cancellationToken = default);


    Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams paginationParams, Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync<T>(PaginationParams paginationParams, Expression<Func<TEntity, T>> mapping, Expression<Func<TEntity, bool>> criteria, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);



    // Any
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);


    // All
    Task<bool> AllAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    // Max
    Task<TResult?> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);
    Task<TResult?> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    // MaxBy
    Task<TEntity?> MaxByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, CancellationToken cancellationToken = default);
    Task<TEntity?> MaxByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>>? criteria = null, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);

    // Min
    Task<TResult?> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);
    Task<TResult?> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    // MinBy
    Task<TEntity?> MinByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, CancellationToken cancellationToken = default);
    Task<TEntity?> MinByAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>>? criteria = null, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);

    // Sum
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<double> SumAsync(Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default);
    Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<float> SumAsync(Expression<Func<TEntity, float>> selector, CancellationToken cancellationToken = default);
    Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<long> SumAsync(Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default);
    Task<long> SumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default);
    Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    // Average
    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<double> AverageAsync(Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default);
    Task<double> AverageAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<float> AverageAsync(Expression<Func<TEntity, float>> selector, CancellationToken cancellationToken = default);
    Task<float> AverageAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<double> AverageAsync(Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default);
    Task<double> AverageAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

    Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default);
    Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);



}

public interface IRepository<TEntity, TId> : IRepository<TEntity>
where TId : IEquatable<TId>
where TEntity : IEntity<TId>
{
    Task<bool> DeleteAsync(TId id, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(TId id, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
    Task<TEntity?> ReadOnlyAsync(TId id, IEnumerable<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
}




