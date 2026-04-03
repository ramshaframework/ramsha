using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;

namespace Ramsha.EntityFrameworkCore
{
    internal static class QueryableExtensions
    {
        internal static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, object>>>? selector)
        where T : class, IEntity
        {
            if (selector != null)
                source = selector.Aggregate(source, (current, include) => current.Include(include.AsPath()));

            return source;
        }
    }
}