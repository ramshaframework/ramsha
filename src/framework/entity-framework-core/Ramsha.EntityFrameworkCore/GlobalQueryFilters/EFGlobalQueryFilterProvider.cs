using System.Collections.Concurrent;
using System.Linq.Expressions;
using Ramsha.Common.Domain;

namespace Ramsha.EntityFrameworkCore;

public abstract class EFGlobalQueryFilterProvider<TDbContext, TFilter> : IEFGlobalQueryFilterProvider<TDbContext>
     where TFilter : class
     where TDbContext : RamshaEFDbContext<TDbContext>
{
    private static readonly ConcurrentDictionary<string, LambdaExpression> _cache = new();
    public Type FilterType => typeof(TFilter);

    protected abstract Expression<Func<TFilter, bool>> CreateFilterExpression(TDbContext dbContext);

    LambdaExpression? IGlobalQueryFilterProvider<TDbContext>.GetFilter(TDbContext dbContext)
    {
        return ExpressionHelpers.CombineExpressions(
            x => !dbContext.IsFilterEnabled<TFilter>(),
             CreateFilterExpression(dbContext),
             ExpressionHelpers.CombineOperator.Or);
    }
}


