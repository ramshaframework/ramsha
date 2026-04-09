using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static IOrderedSpecificationBuilder<T, TResult> OrderBy<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector)
        {
            OrderBy((ISpecificationBuilder<T>)builder, keySelector, true);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> OrderBy<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            OrderBy((ISpecificationBuilder<T>)builder, keySelector, condition);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static IOrderedSpecificationBuilder<T> OrderBy<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector)
            => OrderBy(builder, keySelector, true);

        public static IOrderedSpecificationBuilder<T> OrderBy<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            if (condition)
            {
                var expr = new OrderExpressionInfo<T>(keySelector, OrderTypeEnum.OrderBy);
                builder.Specification.Add(expr);
            }

            Specification<T>.IsChainDiscarded = !condition;
            return (SpecificationBuilder<T>)builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> OrderByDescending<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector)
        {
            OrderByDescending((ISpecificationBuilder<T>)builder, keySelector, true);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> OrderByDescending<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            OrderByDescending((ISpecificationBuilder<T>)builder, keySelector, condition);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector)
            => OrderByDescending(builder, keySelector, true);

        public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            if (condition)
            {
                var expr = new OrderExpressionInfo<T>(keySelector, OrderTypeEnum.OrderByDescending);
                builder.Specification.Add(expr);
            }

            Specification<T>.IsChainDiscarded = !condition;
            return (SpecificationBuilder<T>)builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> ThenBy<T, TResult>(
            this IOrderedSpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector)
        {
            ThenBy((IOrderedSpecificationBuilder<T>)builder, keySelector, true);
            return builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> ThenBy<T, TResult>(
            this IOrderedSpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            ThenBy((IOrderedSpecificationBuilder<T>)builder, keySelector, condition);
            return builder;
        }

        public static IOrderedSpecificationBuilder<T> ThenBy<T>(
            this IOrderedSpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector)
            => ThenBy(builder, keySelector, true);

        public static IOrderedSpecificationBuilder<T> ThenBy<T>(
            this IOrderedSpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            if (condition && !Specification<T>.IsChainDiscarded)
            {
                var expr = new OrderExpressionInfo<T>(keySelector, OrderTypeEnum.ThenBy);
                builder.Specification.Add(expr);
            }
            else
            {
                Specification<T>.IsChainDiscarded = true;
            }

            return builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> ThenByDescending<T, TResult>(
            this IOrderedSpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector)
        {
            ThenByDescending((IOrderedSpecificationBuilder<T>)builder, keySelector, true);
            return builder;
        }

        public static IOrderedSpecificationBuilder<T, TResult> ThenByDescending<T, TResult>(
            this IOrderedSpecificationBuilder<T, TResult> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            ThenByDescending((IOrderedSpecificationBuilder<T>)builder, keySelector, condition);
            return builder;
        }

        public static IOrderedSpecificationBuilder<T> ThenByDescending<T>(
            this IOrderedSpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector)
            => ThenByDescending(builder, keySelector, true);

        public static IOrderedSpecificationBuilder<T> ThenByDescending<T>(
            this IOrderedSpecificationBuilder<T> builder,
            Expression<Func<T, object?>> keySelector,
            bool condition)
        {
            if (condition && !Specification<T>.IsChainDiscarded)
            {
                var expr = new OrderExpressionInfo<T>(keySelector, OrderTypeEnum.ThenByDescending);
                builder.Specification.Add(expr);
            }
            else
            {
                Specification<T>.IsChainDiscarded = true;
            }

            return builder;
        }
    }
}