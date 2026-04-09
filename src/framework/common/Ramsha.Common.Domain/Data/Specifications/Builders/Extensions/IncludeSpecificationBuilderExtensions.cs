using System.Linq.Expressions;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static ISpecificationBuilder<T, TResult> Include<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            string includeString) where T : class
            => Include(builder, includeString, true);

        public static ISpecificationBuilder<T, TResult> Include<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            string includeString,
            bool condition) where T : class
        {
            if (condition)
            {
                builder.Specification.Add(includeString);
            }

            return builder;
        }

        public static ISpecificationBuilder<T> Include<T>(
            this ISpecificationBuilder<T> builder,
            string includeString) where T : class
            => Include(builder, includeString, true);

        public static ISpecificationBuilder<T> Include<T>(
            this ISpecificationBuilder<T> builder,
            string includeString,
            bool condition) where T : class
        {
            if (condition)
            {
                builder.Specification.Add(includeString);
            }

            return builder;
        }

        public static IIncludableSpecificationBuilder<T, TResult, TProperty> Include<T, TResult, TProperty>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, TProperty>> navigationSelector) where T : class
            => Include(builder, navigationSelector, true);

        public static IIncludableSpecificationBuilder<T, TResult, TProperty> Include<T, TResult, TProperty>(
            this ISpecificationBuilder<T, TResult> builder,
            Expression<Func<T, TProperty>> navigationSelector,
            bool condition) where T : class
        {
            if (condition)
            {
                var expr = new IncludeExpressionInfo(navigationSelector);
                builder.Specification.Add(expr);
            }

            Specification<T>.IsChainDiscarded = !condition;
            var includeBuilder = new IncludableSpecificationBuilder<T, TResult, TProperty>(builder.Specification);
            return includeBuilder;
        }

        public static IIncludableSpecificationBuilder<T, TProperty> Include<T, TProperty>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, TProperty>> navigationSelector) where T : class
            => Include(builder, navigationSelector, true);

        public static IIncludableSpecificationBuilder<T, TProperty> Include<T, TProperty>(
            this ISpecificationBuilder<T> builder,
            Expression<Func<T, TProperty>> navigationSelector,
            bool condition) where T : class
        {
            if (condition)
            {
                var expr = new IncludeExpressionInfo(navigationSelector);
                builder.Specification.Add(expr);
            }

            Specification<T>.IsChainDiscarded = !condition;
            var includeBuilder = new IncludableSpecificationBuilder<T, TProperty>(builder.Specification);
            return includeBuilder;
        }

        public static IIncludableSpecificationBuilder<TEntity, TResult, TProperty> ThenInclude<TEntity, TResult, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TResult, TPreviousProperty> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector)
            where TEntity : class
            => ThenInclude(builder, navigationSelector, true);

        public static IIncludableSpecificationBuilder<TEntity, TResult, TProperty> ThenInclude<TEntity, TResult, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TResult, TPreviousProperty> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector,
            bool condition)
            where TEntity : class
        {
            if (condition && !Specification<TEntity>.IsChainDiscarded)
            {
                var expr = new IncludeExpressionInfo(navigationSelector, typeof(TPreviousProperty));
                builder.Specification.Add(expr);
            }
            else
            {
                Specification<TEntity>.IsChainDiscarded = true;
            }

            var includeBuilder = new IncludableSpecificationBuilder<TEntity, TResult, TProperty>(builder.Specification);
            return includeBuilder;
        }

        public static IIncludableSpecificationBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TPreviousProperty> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector)
            where TEntity : class
            => ThenInclude(builder, navigationSelector, true);

        public static IIncludableSpecificationBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TPreviousProperty> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector,
            bool condition)
            where TEntity : class
        {
            if (condition && !Specification<TEntity>.IsChainDiscarded)
            {
                var expr = new IncludeExpressionInfo(navigationSelector, typeof(TPreviousProperty));
                builder.Specification.Add(expr);
            }
            else
            {
                Specification<TEntity>.IsChainDiscarded = true;
            }

            var includeBuilder = new IncludableSpecificationBuilder<TEntity, TProperty>(builder.Specification);
            return includeBuilder;
        }

        public static IIncludableSpecificationBuilder<TEntity, TResult, TProperty> ThenInclude<TEntity, TResult, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TResult, IEnumerable<TPreviousProperty>> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector)
            where TEntity : class
            => ThenInclude(builder, navigationSelector, true);

        public static IIncludableSpecificationBuilder<TEntity, TResult, TProperty> ThenInclude<TEntity, TResult, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TResult, IEnumerable<TPreviousProperty>> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector,
            bool condition)
            where TEntity : class
        {
            if (condition && !Specification<TEntity>.IsChainDiscarded)
            {
                var expr = new IncludeExpressionInfo(navigationSelector, typeof(IEnumerable<TPreviousProperty>));
                builder.Specification.Add(expr);
            }
            else
            {
                Specification<TEntity>.IsChainDiscarded = true;
            }

            var includeBuilder = new IncludableSpecificationBuilder<TEntity, TResult, TProperty>(builder.Specification);
            return includeBuilder;
        }

        public static IIncludableSpecificationBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector)
            where TEntity : class
            => ThenInclude(builder, navigationSelector, true);

        public static IIncludableSpecificationBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationSelector,
            bool condition)
            where TEntity : class
        {
            if (condition && !Specification<TEntity>.IsChainDiscarded)
            {
                var expr = new IncludeExpressionInfo(navigationSelector, typeof(IEnumerable<TPreviousProperty>));
                builder.Specification.Add(expr);
            }
            else
            {
                Specification<TEntity>.IsChainDiscarded = true;
            }

            var includeBuilder = new IncludableSpecificationBuilder<TEntity, TProperty>(builder.Specification);
            return includeBuilder;
        }
    }
}