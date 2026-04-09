using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{

    public static partial class SpecificationBuilderExtensions
    {
        public static ISpecificationBuilder<T, TResult> PostProcessingAction<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Func<IEnumerable<TResult>, IEnumerable<TResult>> filter)
        {
            builder.Specification.PostProcessingAction = filter;
            return builder;
        }

        public static ISpecificationBuilder<T> PostProcessingAction<T>(
            this ISpecificationBuilder<T> builder,
            Func<IEnumerable<T>, IEnumerable<T>> filter)
        {
            builder.Specification.PostProcessingAction = filter;
            return builder;
        }
    }
}