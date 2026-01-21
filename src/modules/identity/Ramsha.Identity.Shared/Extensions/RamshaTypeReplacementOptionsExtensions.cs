

namespace Ramsha.Identity.Shared;

public static class RamshaTypeReplacementOptionsExtensions
{
    public static Type GetIdentityIdOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetIdentityBaseIdType());
    }

    public static RamshaTypeReplacementOptions? ReplaceIdentityId<TId>(this RamshaTypeReplacementOptions options)
    where TId : IEquatable<TId>
    {
        return options.ForceReplace(GetIdentityBaseIdType(), typeof(TId));
    }

    public static RamshaTypeReplacementOptions? ReplaceIdentityId(this RamshaTypeReplacementOptions options, Type idType)
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

        return options.ForceReplace(GetIdentityBaseIdType(), idType);
    }

    private static Type GetIdentityBaseIdType()
    {
        return typeof(Guid);
    }
}
