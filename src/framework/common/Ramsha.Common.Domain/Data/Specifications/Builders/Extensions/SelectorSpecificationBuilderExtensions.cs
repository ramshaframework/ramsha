using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static void Select<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, TResult>> selector)
        {
            builder.Specification.Selector = selector;
        }

        public static void SelectMany<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, IEnumerable<TResult>>> selector)
        {
            builder.Specification.SelectorMany = selector;
        }
    }
}