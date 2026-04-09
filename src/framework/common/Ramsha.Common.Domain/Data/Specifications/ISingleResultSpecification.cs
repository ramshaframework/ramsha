using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    /// <summary>
    /// Encapsulates query logic for <typeparamref name="T"/>. It is meant to return a single result.
    /// </summary>
    /// <typeparam name="T">The type being queried against.</typeparam>
    public interface ISingleResultSpecification<T> : ISpecification<T> where T : IEntity//, ISingleResultSpecification
    {
    }

    /// <summary>
    /// Encapsulates query logic for <typeparamref name="T"/>,
    /// and projects the result into <typeparamref name="TResult"/>. It is meant to return a single result.
    /// </summary>
    /// <typeparam name="T">The type being queried against.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface ISingleResultSpecification<T, TResult> : ISpecification<T, TResult> where T : IEntity//, ISingleResultSpecification
    {
    }

    public class SingleResultSpecification<T> : Specification<T>, ISingleResultSpecification<T> where T : IEntity
    {
    }

    /// <inheritdoc cref="ISingleResultSpecification{T, TResult}"/>
    public class SingleResultSpecification<T, TResult> : Specification<T, TResult>, ISingleResultSpecification<T, TResult> where T : IEntity
    {
    }
}