using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public class InMemorySpecificationEvaluator : IInMemorySpecificationEvaluator
    {
        public static InMemorySpecificationEvaluator Default { get; } = new InMemorySpecificationEvaluator();

        protected List<IInMemoryEvaluator> Evaluators { get; }

        public InMemorySpecificationEvaluator()
        {
            Evaluators =
            [
                WhereEvaluator.Instance,
                SearchMemoryEvaluator.Instance,
                OrderEvaluator.Instance,
                PaginationEvaluator.Instance
            ];
        }

        public InMemorySpecificationEvaluator(IEnumerable<IInMemoryEvaluator> evaluators)
        {
            Evaluators = evaluators.ToList();
        }

        public virtual IEnumerable<TResult> Evaluate<T, TResult>(IEnumerable<T> source, ISpecification<T, TResult> specification)
        {
            if (specification.Selector is null && specification.SelectorMany is null) throw new SelectorNotFoundException();
            if (specification.Selector != null && specification.SelectorMany != null) throw new ConcurrentSelectorsException();

            var baseQuery = Evaluate(source, (ISpecification<T>)specification);

            var resultQuery = specification.Selector != null
              ? baseQuery.Select(specification.Selector.Compile())
              : baseQuery.SelectMany(specification.SelectorMany!.Compile());

            return specification.PostProcessingAction is null
                ? resultQuery
                : specification.PostProcessingAction(resultQuery);
        }

        public virtual IEnumerable<T> Evaluate<T>(IEnumerable<T> source, ISpecification<T> specification)
        {
            foreach (var evaluator in Evaluators)
            {
                source = evaluator.Evaluate(source, specification);
            }

            return specification.PostProcessingAction is null
                ? source
                : specification.PostProcessingAction(source);
        }
    }

    public interface IInMemoryEvaluator
    {
        IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification);
    }

    public interface IInMemorySpecificationEvaluator
    {
        IEnumerable<TResult> Evaluate<T, TResult>(IEnumerable<T> source, ISpecification<T, TResult> specification);
        IEnumerable<T> Evaluate<T>(IEnumerable<T> source, ISpecification<T> specification);
    }

    public interface ISpecificationEvaluator
    {
        IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> specification) where T : class;
        IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> specification, bool evaluateCriteriaOnly = false) where T : class;
    }
}