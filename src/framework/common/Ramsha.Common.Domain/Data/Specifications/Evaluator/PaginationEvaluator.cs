namespace Ramsha.Common.Domain
{
    public class PaginationEvaluator : IEvaluator, IInMemoryEvaluator
    {

        public static PaginationEvaluator Instance { get; } = new PaginationEvaluator();
        private PaginationEvaluator() { }

        public bool IsCriteriaEvaluator { get; } = false;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.Skip > 0)
            {
                query = query.Skip(specification.Skip);
            }

            if (specification.Take >= 0)
            {
                query = query.Take(specification.Take);
            }

            return query;
        }

        public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification)
        {
            if (specification.Skip > 0)
            {
                query = query.Skip(specification.Skip);
            }

            if (specification.Take >= 0)
            {
                query = query.Take(specification.Take);
            }

            return query;
        }
    }
}