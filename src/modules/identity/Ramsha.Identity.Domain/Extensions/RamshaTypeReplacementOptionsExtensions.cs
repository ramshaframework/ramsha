using Ramsha.Common.Domain;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Domain;

public static class RamshaTypeReplacementOptionsExtensions
{

    public static RamshaIdentityEntitiesTypes GetRamshaIdentityEntitiesTypes(this RamshaTypeReplacementOptions options)
    {
        return new RamshaIdentityEntitiesTypes(
            options.GetUserTypeOrBase(),
            options.GetRoleTypeOrBase(),
            options.GetUserRoleTypeOrBase(),
            options.GetRoleClaimTypeOrBase(),
            options.GetUserClaimTypeOrBase(),
            options.GetUserLoginTypeOrBase(),
            options.GetUserTokenTypeOrBase()
        );
    }

    public record RamshaIdentityEntitiesTypes(
     Type UserType,
     Type RoleType,
     Type UserRoleType,
     Type RoleClaimType,
     Type UserClaimType,
     Type UserLoginType,
     Type UserTokenType
    );

    public static Type GetUserRoleTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseUserRoleType());
    }

    public static Type GetUserClaimTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseUserClaimType());
    }

    public static Type GetUserLoginTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseUserLoginType());
    }
    public static Type GetUserTokenTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseUserTokenType());
    }

    public static Type GetRoleClaimTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseRoleClaimType());
    }

    public static Type GetRoleTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseRoleType());
    }

    public static Type GetUserTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrBase(GetBaseUserType());
    }

    public static RamshaTypeReplacementOptions ReplaceRole<TRole>(this RamshaTypeReplacementOptions options)
    {
        options.ForceReplace(GetBaseRoleType(), typeof(TRole));
        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceUser<TUser>(this RamshaTypeReplacementOptions options)
    {
        ReplaceUser(options, typeof(TUser));
        return options;
    }



    public static RamshaTypeReplacementOptions ReplaceRole(this RamshaTypeReplacementOptions options, Type roleType)
    {
        options.ForceReplace(GetBaseRoleType(), roleType);
        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceUser(this RamshaTypeReplacementOptions options, Type userType)
    {
        var userIdType = FoundAndValidateIdentityEntityId(userType, options);
        var expectedBase = typeof(RamshaIdentityUserBase<>)
        .MakeGenericType(userIdType);

        if (!expectedBase.IsAssignableFrom(userType))
        {
            throw new RamshaException("Invalid identity user type");
        }
        options.ForceReplace(GetBaseUserType(), userType);
        return options;
    }

    private static Type FoundAndValidateIdentityEntityId(Type entityType, RamshaTypeReplacementOptions options)
    {
        var entityIdType = EntityHelper.FindPrimaryKeyType(entityType);

        if (entityIdType is null)
        {
            throw new RamshaException(
                $"Unable to determine the primary key type for entity '{entityType.FullName}'. " +
                $"Identity entities must define a valid primary key.");
        }

        var identityIdType = options.GetIdentityIdOrBase();

        if (identityIdType is not null)
        {
            if (entityIdType != identityIdType)
            {
                throw new RamshaException(
                    $"Identity key type mismatch for entity '{entityType.FullName}'. " +
                    $"Expected '{identityIdType.Name}' but found '{entityIdType.Name}'. " +
                    $"All identity entities must use the same key type.");
            }
        }
        else
        {
            options.ReplaceIdentityId(entityIdType);
        }

        return entityIdType;
    }

    public static RamshaTypeReplacementOptions ReplaceUserRole<TUserRole, TId>(this RamshaTypeReplacementOptions options)
    where TId : IEquatable<TId>
    where TUserRole : RamshaIdentityUserRole<TId>
    {
        options.ForceReplace(GetBaseUserRoleType(), typeof(TUserRole));
        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceUserClaim<TUserClaim, TId>(this RamshaTypeReplacementOptions options)
    where TId : IEquatable<TId>
    where TUserClaim : RamshaIdentityUserClaim<TId>
    {
        options.ForceReplace(GetBaseUserClaimType(), typeof(TUserClaim));
        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceUserLogin<TUserLogin, TId>(this RamshaTypeReplacementOptions options)
   where TId : IEquatable<TId>
   where TUserLogin : RamshaIdentityUserLogin<TId>
    {
        options.ForceReplace(GetBaseUserLoginType(), typeof(TUserLogin));
        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceUserToken<TUserToken, TId>(this RamshaTypeReplacementOptions options)
   where TId : IEquatable<TId>
   where TUserToken : RamshaIdentityUserToken<TId>
    {
        options.ForceReplace(GetBaseUserTokenType(), typeof(TUserToken));
        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceRoleClaim<TRoleClaim, TId>(this RamshaTypeReplacementOptions options)
   where TId : IEquatable<TId>
   where TRoleClaim : RamshaIdentityRoleClaim<TId>
    {
        options.ForceReplace(GetBaseRoleClaimType(), typeof(TRoleClaim));
        return options;
    }

    private static Type GetBaseUserType()
    {
        return typeof(RamshaIdentityUser);
    }

    private static Type GetBaseRoleType()
    {
        return typeof(RamshaIdentityRole);
    }

    private static Type GetBaseUserRoleType()
    {
        return typeof(RamshaIdentityUserRole<Guid>);
    }

    private static Type GetBaseUserClaimType()
    {
        return typeof(RamshaIdentityUserClaim<Guid>);
    }

    private static Type GetBaseUserTokenType()
    {
        return typeof(RamshaIdentityUserToken<Guid>);
    }

    private static Type GetBaseUserLoginType()
    {
        return typeof(RamshaIdentityUserLogin<Guid>);
    }

    private static Type GetBaseRoleClaimType()
    {
        return typeof(RamshaIdentityRoleClaim<Guid>);
    }


}
