namespace Ramsha.Common.Domain
{
    public class SearchValidator : IValidator
    {
        private SearchValidator() { }
        public static SearchValidator Instance { get; } = new SearchValidator();

        public bool IsValid<T>(T entity, ISpecification<T> specification)
        {
            if (specification is Specification<T> spec)
            {
                if (spec.OneOrManySearchExpressions.IsEmpty) return true;

                if (spec.OneOrManySearchExpressions.SingleOrDefault is { } searchExpression)
                {
                    return searchExpression.SelectorFunc(entity)?.Like(searchExpression.SearchTerm) ?? false;
                }

                return IsValid(entity, spec.OneOrManySearchExpressions.List);
            }



            return Fallback(entity, specification);

            static bool Fallback(T entity, ISpecification<T> specification)
            {
                foreach (var searchGroup in specification.SearchCriteria.GroupBy(x => x.SearchGroup))
                {
                    if (!searchGroup.Any(c => c.SelectorFunc(entity)?.Like(c.SearchTerm) ?? false))
                        return false;
                }

                return true;
            }
        }


        private static bool IsValid<T>(T entity, List<SearchExpressionInfo<T>> list)
        {
            var groupStart = 0;
            for (var i = 1; i <= list.Count; i++)
            {
                if (i == list.Count || list[i].SearchGroup != list[groupStart].SearchGroup)
                {
                    if (IsValidInOrGroup(entity, list, groupStart, i) is false)
                    {
                        return false;
                    }
                    groupStart = i;
                }
            }
            return true;

            static bool IsValidInOrGroup(T sourceItem, List<SearchExpressionInfo<T>> list, int from, int to)
            {
                var validOrGroup = false;
                for (int i = from; i < to; i++)
                {
                    if (list[i].SelectorFunc(sourceItem)?.Like(list[i].SearchTerm) ?? false)
                    {
                        validOrGroup = true;
                        break;
                    }
                }
                return validOrGroup;
            }
        }
    }
}