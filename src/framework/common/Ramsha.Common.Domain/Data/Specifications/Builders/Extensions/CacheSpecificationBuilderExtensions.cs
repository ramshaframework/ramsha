using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public static partial class SpecificationBuilderExtensions
    {
        public static ICacheSpecificationBuilder<T, TResult> EnableCache<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            string specificationName,
            params object[] args) where T : class
        {
            EnableCache((ISpecificationBuilder<T>)builder, specificationName, true, args);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static ICacheSpecificationBuilder<T, TResult> EnableCache<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            string specificationName,
            bool condition,
            params object[] args) where T : class
        {
            EnableCache((ISpecificationBuilder<T>)builder, specificationName, condition, args);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static ICacheSpecificationBuilder<T> EnableCache<T>(
            this ISpecificationBuilder<T> builder,
            string specificationName,
            params object[] args) where T : class
            => EnableCache(builder, specificationName, true, args);

        public static ICacheSpecificationBuilder<T> EnableCache<T>(
            this ISpecificationBuilder<T> builder,
            string specificationName,
            bool condition,
            params object[] args) where T : class
        {
            if (condition)
            {
                if (string.IsNullOrEmpty(specificationName))
                {
                    throw new ArgumentException("Required input was null or empty.", nameof(specificationName));
                }

                builder.Specification.CacheKey = $"{specificationName}-{string.Join("-", args)}";
            }

            Specification<T>.IsChainDiscarded = !condition;
            return (SpecificationBuilder<T>)builder;
        }

        public static ICacheSpecificationBuilder<T, TResult> WithCacheKey<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            string cacheKey) where T : class
        {
            WithCacheKey((ISpecificationBuilder<T>)builder, cacheKey, true);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static ICacheSpecificationBuilder<T, TResult> WithCacheKey<T, TResult>(
            this ISpecificationBuilder<T, TResult> builder,
            string cacheKey,
            bool condition) where T : class
        {
            WithCacheKey((ISpecificationBuilder<T>)builder, cacheKey, condition);
            return (SpecificationBuilder<T, TResult>)builder;
        }

        public static ICacheSpecificationBuilder<T> WithCacheKey<T>(
            this ISpecificationBuilder<T> builder,
            string cacheKey) where T : class
            => WithCacheKey(builder, cacheKey, true);

        public static ICacheSpecificationBuilder<T> WithCacheKey<T>(
            this ISpecificationBuilder<T> builder,
            string cacheKey,
            bool condition) where T : class
        {
            if (condition)
            {
                if (string.IsNullOrEmpty(cacheKey))
                {
                    throw new ArgumentException("Required input was null or empty.", nameof(cacheKey));
                }

                builder.Specification.CacheKey = cacheKey;
            }

            Specification<T>.IsChainDiscarded = !condition;
            return (SpecificationBuilder<T>)builder;
        }
    }
}