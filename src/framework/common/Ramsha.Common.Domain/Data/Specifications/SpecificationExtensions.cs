namespace Ramsha.Common.Domain
{
    public static class SpecificationExtensions
    {

        public static Specification<T, TResult> WithProjectionOf<T, TResult>(this Specification<T> source, Specification<T, TResult> projectionSpec) where T : IEntity
        {
            var newSpec = source.Clone<TResult>();
            newSpec.Selector = projectionSpec.Selector;
            newSpec.SelectorMany = projectionSpec.SelectorMany;
            newSpec.PostProcessingAction = projectionSpec.PostProcessingAction;
            return newSpec;
        }
    }





}