using System.Reflection;
using System.Linq.Expressions;
using Ramsha.Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ramsha.EntityFrameworkCore
{
    internal static class IncludeExtensions
    {
        internal static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, object>>>? selector)
        where T : class, IEntity
        {
            if (selector != null)
                source = selector.Aggregate(source, (current, include) => current.Include(include.AsPath()));

            return source;
        }

        internal static IQueryable<T> Include<T>(this IQueryable<T> source, IncludeExpressionInfo info)
        where T : class, IEntity
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));
            var propertyName = GetPropertyName(info.LambdaExpression);

            return EntityFrameworkQueryableExtensions.Include(source, propertyName);
        }

        internal static IQueryable<T> ThenInclude<T>(this IQueryable<T> source, IncludeExpressionInfo info)
        where T : class, IEntity
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));

            var exp = source.Expression as MethodCallExpression;
            var arg = exp.Arguments[0] as ConstantExpression;

            string previousPropertyName;
            if (arg.Value is string)
            {
                previousPropertyName = arg.Value.ToString();
            }
            else
            {
                // System.Data.Entity.Core.Objects.Span is an internal class, so here's some reflection to get to the previous property.

                var propertyInfo = arg.Value.GetType().GetProperty("SpanList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var spanList = propertyInfo.GetValue(arg.Value);

                // Get the first item of the span list
                propertyInfo = propertyInfo.PropertyType.GetProperty("Item");
                var spanPath = propertyInfo.GetValue(spanList, new object[] { 0 });

                var fieldInfo = spanPath.GetType().GetField("Navigations", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var navigations = fieldInfo.GetValue(spanPath) as List<string>;
                previousPropertyName = string.Join(".", navigations);
            }

            var propertyName = GetPropertyName(info.LambdaExpression);

            return EntityFrameworkQueryableExtensions.Include(source, $"{previousPropertyName}.{propertyName}");
        }

        private static string GetPropertyName(this LambdaExpression propertySelector, char delimiter = '.', char endTrim = ')')
        {

            var asString = propertySelector.ToString();
            var firstDelim = asString.IndexOf(delimiter);

            return firstDelim < 0
                ? asString
                : asString.Substring(firstDelim + 1).TrimEnd(endTrim);
        }
    }
}