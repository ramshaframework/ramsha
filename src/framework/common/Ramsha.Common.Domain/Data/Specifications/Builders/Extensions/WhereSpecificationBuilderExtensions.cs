using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static ISpecificationBuilder<T, TResult> Where<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, bool>> predicate)
        {
            Where((ISpecificationBuilder<T>)builder, predicate, true);
            return builder;
        }

        public static ISpecificationBuilder<T, TResult> Where<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, bool>> predicate,
            bool condition)
        {
            Where((ISpecificationBuilder<T>)builder, predicate, condition);
            return builder;
        }

        public static ISpecificationBuilder<T> Where<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, bool>> predicate)
            => Where(builder, predicate, true);

        public static ISpecificationBuilder<T> Where<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, bool>> predicate,
            bool condition)
        {
            if (condition)
            {
                var expr = new WhereExpressionInfo<T>(predicate);
                builder.Specification.Add(expr);
            }

            return builder;
        }
    }
}