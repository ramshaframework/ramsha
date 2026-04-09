using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain;

public static partial class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T, TResult> TagWith<T, TResult>(
        this ISpecificationBuilder<T, TResult> builder,
        string tag)
    {
        TagWith((ISpecificationBuilder<T>)builder, tag, true);
        return builder;
    }

    public static ISpecificationBuilder<T, TResult> TagWith<T, TResult>(
        this ISpecificationBuilder<T, TResult> builder,
        string tag,
        bool condition)
    {
        TagWith((ISpecificationBuilder<T>)builder, tag, condition);
        return builder;
    }

    public static ISpecificationBuilder<T> TagWith<T>(
        this ISpecificationBuilder<T> builder,
        string tag)
        => TagWith(builder, tag, true);

    public static ISpecificationBuilder<T> TagWith<T>(
        this ISpecificationBuilder<T> builder,
        string tag,
        bool condition)
    {
        if (condition)
        {
            builder.Specification.AddQueryTag(tag);
        }

        return builder;
    }
}

