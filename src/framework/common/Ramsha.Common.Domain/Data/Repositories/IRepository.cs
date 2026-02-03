using System.Linq.Expressions;

namespace Ramsha.Common.Domain;

public interface IRepository
{

}

public interface IRepository<TEntity> : IRepository
where TEntity : IEntity
{
    Task<int> GetCountAsync();
    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> criteria);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);

    Task<TEntity?> AddAsync(TEntity entity);
    Task<List<TEntity>> GetListAsync();
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> criteria, params Expression<Func<TEntity, object>>[] includes);
    Task<List<TEntity>> GetListAsync(params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> criteria, params Expression<Func<TEntity, object>>[] includes);

    Task<IQueryable<TEntity>> QueryAsync();

}

public interface IRepository<TEntity, TId> : IRepository<TEntity>
where TId : IEquatable<TId>
where TEntity : IEntity<TId>
{
    Task<bool> DeleteAsync(TId id);
    Task<bool> IsExist(TId id);
    Task<TEntity?> FindAsync(TId id);
    Task<TEntity?> FindAsync(TId id, params Expression<Func<TEntity, object>>[] includes);

}




