using Ramsha.Common.Domain;

namespace Ramsha.EntityFrameworkCore
{
    public class EfCoreSpecificationEvaluator : ISpecificationEvaluator
    {

        public static EfCoreSpecificationEvaluator Default { get; } = new EfCoreSpecificationEvaluator();

        protected List<IEvaluator> Evaluators { get; }

        public EfCoreSpecificationEvaluator()
        {
            Evaluators =
            [
            WhereEvaluator.Instance,
            SearchEvaluator.Instance,
            IncludeStringEvaluator.Instance,
            IncludeEvaluator.Instance,
            OrderEvaluator.Instance,
            PaginationEvaluator.Instance,
            QueryTagEvaluator.Instance,
        ];
        }

        public EfCoreSpecificationEvaluator(IEnumerable<IEvaluator> evaluators)
        {
            Evaluators = evaluators.ToList();
        }

        public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
        {
            ArgumentNullException.ThrowIfNull(specification);
            if (specification.Selector is null && specification.SelectorMany is null) throw new SelectorNotFoundException();
            if (specification.Selector is not null && specification.SelectorMany is not null) throw new ConcurrentSelectorsException();

            query = GetQuery(query, (ISpecification<T>)specification);

            return specification.Selector is not null
              ? query.Select(specification.Selector)
              : query.SelectMany(specification.SelectorMany!);
        }

        public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification, bool evaluateCriteriaOnly = false) where T : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            var evaluators = evaluateCriteriaOnly ? Evaluators.Where(x => x.IsCriteriaEvaluator) : Evaluators;

            foreach (var evaluator in evaluators)
            {
                query = evaluator.GetQuery(query, specification);
            }

            return query;
        }
    }
}