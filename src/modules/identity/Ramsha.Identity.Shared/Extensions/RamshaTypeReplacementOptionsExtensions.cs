

namespace Ramsha.Identity.Shared;

public static class RamshaTypeReplacementOptionsExtensions
{
    public static Type GetIdentityIdOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrNull(typeof(RamshaIdentityIdTypeTypedKey)) ?? GetIdentityBaseIdType();
    }

    internal static RamshaTypeReplacementOptions? ReplaceIdentityId<TId>(this RamshaTypeReplacementOptions options)
    where TId : IEquatable<TId>
    {
        return options.Replace(typeof(RamshaIdentityIdTypeTypedKey), typeof(TId));
    }

    internal static RamshaTypeReplacementOptions? ReplaceIdentityId(this RamshaTypeReplacementOptions options, Type idType)
    {
        if (idType == null)
            throw new ArgumentNullException(nameof(idType));

        var equatableType = typeof(IEquatable<>).MakeGenericType(idType);

        if (!equatableType.IsAssignableFrom(idType))
        {
            throw new RamshaException(
                $"Identity id type '{idType.Name}' must implement " +
                $"IEquatable<{idType.Name}>.");
        }

        return options.Replace(typeof(RamshaIdentityIdTypeTypedKey), idType);
    }

    private static Type GetIdentityBaseIdType()
    {
        return typeof(Guid);
    }
}


internal class RamshaIdentityIdTypeTypedKey : IRamshaTypedKey { };
