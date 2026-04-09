using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static ISpecificationBuilder<T, TResult> Search<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, string?>> keySelector,
            string pattern,
            int group = 1) where T : class
        {
            Search((ISpecificationBuilder<T>)builder, keySelector, pattern, true, group);
            return builder;
        }

        public static ISpecificationBuilder<T, TResult> Search<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, string?>> keySelector,
            string pattern,
            bool condition,
            int group = 1) where T : class
        {
            Search((ISpecificationBuilder<T>)builder, keySelector, pattern, condition, group);
            return builder;
        }

        public static ISpecificationBuilder<T> Search<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, string?>> keySelector,
            string pattern,
            int group = 1) where T : class
            => Search(builder, keySelector, pattern, true, group);

        public static ISpecificationBuilder<T> Search<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, string?>> keySelector,
            string pattern,
            bool condition,
            int group = 1) where T : class
        {
            if (condition)
            {
                var expr = new SearchExpressionInfo<T>(keySelector, pattern, group);
                builder.Specification.Add(expr);
            }

            return builder;
        }
    }

}