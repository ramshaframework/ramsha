namespace Ramsha.Common.Domain
{
    public class WhereEvaluator : IEvaluator, IInMemoryEvaluator
    {
        public static WhereEvaluator Instance { get; } = new WhereEvaluator();
        private WhereEvaluator() { }

        public bool IsCriteriaEvaluator { get; } = true;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification is Specification<T> spec)
            {
                if (spec.OneOrManyWhereExpressions.IsEmpty) return query;
                if (spec.OneOrManyWhereExpressions.SingleOrDefault is { } whereExpression)
                {
                    return query.Where(whereExpression.Filter);
                }

                foreach (var whereExpr in spec.OneOrManyWhereExpressions.List)
                {
                    query = query.Where(whereExpr.Filter);
                }
                return query;
            }

            foreach (var whereExpression in specification.WhereExpressions)
            {
                query = query.Where(whereExpression.Filter);
            }
            return query;
        }

        public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification)
        {
            if (specification is Specification<T> spec)
            {
                if (spec.OneOrManyWhereExpressions.IsEmpty) return query;
                if (spec.OneOrManyWhereExpressions.SingleOrDefault is { } whereExpression)
                {
                    return query.Where(whereExpression.FilterFunc);
                }

                foreach (var whereExpr in spec.OneOrManyWhereExpressions.List)
                {
                    query = query.Where(whereExpr.FilterFunc);
                }
                return query;
            }

            foreach (var whereExpression in specification.WhereExpressions)
            {
                query = query.Where(whereExpression.FilterFunc);
            }
            return query;
        }
    }
}