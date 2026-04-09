using System.ComponentModel;
using System.Linq.Expressions;

namespace Ramsha.Common.Domain
{
    public class Specification<T> : ISpecification<T>
    {
        [ThreadStatic]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsChainDiscarded;

        private OneOrMany<WhereExpressionInfo<T>> _whereExpressions = new();
        private OneOrMany<SearchExpressionInfo<T>> _searchExpressions = new();
        private OneOrMany<OrderExpressionInfo<T>> _orderExpressions = new();
        private OneOrMany<IncludeExpressionInfo> _includeExpressions = new();
        private OneOrMany<string> _includeStrings = new();
        private OneOrMany<string> _queryTags = new();
        private Dictionary<string, object>? _items;

        public ISpecificationBuilder<T> Query => _builder;
        private readonly ISpecificationBuilder<T> _builder;

        public Specification()
        {
            _builder = new SpecificationBuilder<T>(this);
        }

        internal Specification(Action<ISpecificationBuilder<T>> queryAction)
        {
            var builder = new SpecificationBuilder<T>(this);
            queryAction(builder);
            _builder = builder;
        }

        protected virtual IInMemorySpecificationEvaluator Evaluator => InMemorySpecificationEvaluator.Default;
        protected virtual ISpecificationValidator Validator => SpecificationValidator.Default;

        public virtual IEnumerable<T> Evaluate(IEnumerable<T> entities)
        {
            var evaluator = Evaluator;
            return evaluator.Evaluate(entities, this);
        }

        public virtual bool IsSatisfiedBy(T entity)
        {
            var validator = Validator;
            return validator.IsValid(entity, this);
        }

        public Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; internal set; }
        public string? CacheKey { get; internal set; }
        public bool CacheEnabled => CacheKey is not null;
        public int Take { get; internal set; } = -1;
        public int Skip { get; internal set; } = -1;

        public Dictionary<string, object> Items => _items ??= [];

        public IEnumerable<WhereExpressionInfo<T>> WhereExpressions => _whereExpressions.Values;
        public IEnumerable<SearchExpressionInfo<T>> SearchCriteria => _searchExpressions.Values;
        public IEnumerable<OrderExpressionInfo<T>> OrderExpressions => _orderExpressions.Values;
        public IEnumerable<IncludeExpressionInfo> IncludeExpressions => _includeExpressions.Values;
        public IEnumerable<string> IncludeStrings => _includeStrings.Values;
        public IEnumerable<string> QueryTags => _queryTags.Values;

        internal OneOrMany<WhereExpressionInfo<T>> OneOrManyWhereExpressions => _whereExpressions;
        internal OneOrMany<SearchExpressionInfo<T>> OneOrManySearchExpressions => _searchExpressions;
        internal OneOrMany<OrderExpressionInfo<T>> OneOrManyOrderExpressions => _orderExpressions;
        internal OneOrMany<IncludeExpressionInfo> OneOrManyIncludeExpressions => _includeExpressions;
        internal OneOrMany<string> OneOrManyIncludeStrings => _includeStrings;
        internal OneOrMany<string> OneOrManyQueryTags => _queryTags;

        internal void Add(WhereExpressionInfo<T> whereExpression) => _whereExpressions.Add(whereExpression);
        internal void Add(SearchExpressionInfo<T> searchExpression) => _searchExpressions.AddSorted(searchExpression, SearchExpressionComparer<T>.Default);
        internal void Add(OrderExpressionInfo<T> orderExpression) => _orderExpressions.Add(orderExpression);
        internal void Add(IncludeExpressionInfo includeExpression) => _includeExpressions.Add(includeExpression);
        internal void Add(string includeString) => _includeStrings.Add(includeString);
        internal void AddQueryTag(string queryTag) => _queryTags.Add(queryTag);

        internal Specification<T> Clone()
        {
            var newSpec = new Specification<T>();
            CopyState(this, newSpec);
            return newSpec;
        }

        internal Specification<T, TResult> Clone<TResult>()
        {
            var newSpec = new Specification<T, TResult>();
            CopyState(this, newSpec);
            return newSpec;
        }

        private static void CopyState(Specification<T> source, Specification<T> target)
        {
            target.PostProcessingAction = source.PostProcessingAction;
            target.CacheKey = source.CacheKey;
            target.Take = source.Take;
            target.Skip = source.Skip;
            target._whereExpressions = source._whereExpressions.Clone();
            target._searchExpressions = source._searchExpressions.Clone();
            target._orderExpressions = source._orderExpressions.Clone();
            target._includeExpressions = source._includeExpressions.Clone();
            target._includeStrings = source._includeStrings.Clone();
            target._queryTags = source._queryTags.Clone();
            if (source._items is not null)
            {
                target._items = new Dictionary<string, object>(source._items);
            }
        }
    }

    public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
    {
        public new ISpecificationBuilder<T, TResult> Query => new SpecificationBuilder<T, TResult>(this);
        public Expression<Func<T, TResult>>? Selector { get; internal set; }
        public Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; internal set; }
        public new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; internal set; } = null;

        public new virtual IEnumerable<TResult> Evaluate(IEnumerable<T> entities)
        {
            var evaluator = Evaluator;
            return evaluator.Evaluate(entities, this);
        }
    }
}