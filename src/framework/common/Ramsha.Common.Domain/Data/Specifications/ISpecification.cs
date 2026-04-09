using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        new ISpecificationBuilder<T, TResult> Query { get; }
        Expression<Func<T, TResult>>? Selector { get; }
        Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; }
        new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; }
        new IEnumerable<TResult> Evaluate(IEnumerable<T> entities);
    }

    public interface ISpecification<T>
    {
        ISpecificationBuilder<T> Query { get; }
        Dictionary<string, object> Items { get; }
        IEnumerable<WhereExpressionInfo<T>> WhereExpressions { get; }
        IEnumerable<OrderExpressionInfo<T>> OrderExpressions { get; }
        IEnumerable<IncludeExpressionInfo> IncludeExpressions { get; }
        IEnumerable<string> IncludeStrings { get; }
        IEnumerable<SearchExpressionInfo<T>> SearchCriteria { get; }
        int Take { get; }
        int Skip { get; }
        Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; }
        IEnumerable<string> QueryTags { get; }
        bool CacheEnabled { get; }
        string? CacheKey { get; }
        IEnumerable<T> Evaluate(IEnumerable<T> entities);
        bool IsSatisfiedBy(T entity);
    }



}