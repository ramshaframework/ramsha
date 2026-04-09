using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static ISpecificationBuilder<T, TResult> Take<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            int take)
        {
            Take((ISpecificationBuilder<T>)builder, take, true);
            return builder;
        }

        public static ISpecificationBuilder<T, TResult> Take<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            int take,
            bool condition)
        {
            Take((ISpecificationBuilder<T>)builder, take, condition);
            return builder;
        }

        public static ISpecificationBuilder<T> Take<T>(
            this ISpecificationBuilder<T> builder,
            int take)
            => Take(builder, take, true);

        public static ISpecificationBuilder<T> Take<T>(
            this ISpecificationBuilder<T> builder,
            int take,
            bool condition)
        {
            if (condition)
            {
                if (builder.Specification.Take != -1) throw new DuplicateTakeException();
                builder.Specification.Take = take;
            }

            return builder;
        }

        public static ISpecificationBuilder<T, TResult> Skip<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            int skip)
        {
            Skip((ISpecificationBuilder<T>)builder, skip, true);
            return builder;
        }

        public static ISpecificationBuilder<T, TResult> Skip<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            int skip,
            bool condition)
        {
            Skip((ISpecificationBuilder<T>)builder, skip, condition);
            return builder;
        }

        public static ISpecificationBuilder<T> Skip<T>(
            this ISpecificationBuilder<T> builder,
            int skip)
            => Skip(builder, skip, true);

        public static ISpecificationBuilder<T> Skip<T>(
            this ISpecificationBuilder<T> builder,
            int skip,
            bool condition)
        {
            if (condition)
            {
                if (builder.Specification.Skip != -1) throw new DuplicateSkipException();
                builder.Specification.Skip = skip;
            }

            return builder;
        }
    }
}