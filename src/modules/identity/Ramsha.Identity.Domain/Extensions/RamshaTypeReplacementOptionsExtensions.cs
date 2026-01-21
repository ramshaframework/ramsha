using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Domain;

public static class RamshaTypeReplacementOptionsExtensions
{
    public static RamshaTypeReplacementOptions? ReplaceIdentityEntities<TUser>(this RamshaTypeReplacementOptions options)
    where TUser : RamshaIdentityUser, new()
    {
        options.ReplaceUser<TUser>();
        return options;
    }

    public static RamshaTypeReplacementOptions? ReplaceIdentityEntities<TUser, TRole>(this RamshaTypeReplacementOptions options)
    where TUser : RamshaIdentityUser, new()
    where TRole : RamshaIdentityRole, new()
    {
        options.ReplaceUser<TUser>();
        options.ReplaceRole<TRole>();
        return options;
    }

    public static RamshaTypeReplacementOptions? ReplaceIdentityEntities<TUser, TRole, TId>(this RamshaTypeReplacementOptions options)
    where TId : IEquatable<TId>
    where TUser : RamshaIdentityUser<TId>, new()
    where TRole : RamshaIdentityRole<TId>, new()
    {
        options.ReplaceIdentityId<TId>();
        options.ReplaceUser<TUser>();
        options.ReplaceRole<TRole>();
        return options;
    }

    public static RamshaTypeReplacementOptions? ReplaceIdentityEntities<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken>(this RamshaTypeReplacementOptions options)
     where TId : IEquatable<TId>
     where TUser : RamshaIdentityUser<TId, TUserClaim, TUserRole, TUserLogin, TUserToken>, new()
     where TUserClaim : RamshaIdentityUserClaim<TId>, new()
     where TUserRole : RamshaIdentityUserRole<TId>, new()
     where TUserLogin : RamshaIdentityUserLogin<TId>, new()
     where TUserToken : RamshaIdentityUserToken<TId>, new()
     where TRole : RamshaIdentityRole<TId, TUserRole, TRoleClaim>, new()
     where TRoleClaim : RamshaIdentityRoleClaim<TId>, new()
    {
        options.ReplaceIdentityId<TId>();
        options.ReplaceUser<TUser>();
        options.ReplaceRole<TRole>();
        options.ReplaceUserRole<TUserRole>();
        options.ReplaceRoleClaim<TRoleClaim>();
        options.ReplaceUserClaim<TUserClaim>();
        options.ReplaceUserLogin<TUserLogin>();
        options.ReplaceUserToken<TUserToken>();

        return options;
    }




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
        return options.GetOrSelf(GetBaseUserRoleType(options.GetIdentityIdOrBase()));
    }

    public static Type GetUserClaimTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrSelf(GetBaseUserClaimType(options.GetIdentityIdOrBase()));
    }

    public static Type GetUserLoginTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrNull(typeof(RamshaUserLoginTypeTypedKey)) ?? GetBaseUserLoginType(options.GetIdentityIdOrBase());
    }
    public static Type GetUserTokenTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrNull(typeof(RamshaUserTokenTypeTypedKey)) ?? GetBaseUserTokenType(options.GetIdentityIdOrBase());
    }

    public static Type GetRoleClaimTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrNull(typeof(RamshaRoleClaimTypeTypedKey)) ?? GetBaseRoleClaimType(options.GetIdentityIdOrBase());
    }

    public static Type GetRoleTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrNull(typeof(RamshaRoleTypeTypedKey)) ?? GetBaseRoleType();
    }

    public static Type GetUserTypeOrBase(this RamshaTypeReplacementOptions options)
    {
        return options.GetOrNull(typeof(RamshaUserTypeTypedKey)) ?? GetBaseUserType();
    }

    private static RamshaTypeReplacementOptions ReplaceRole<TRole>(this RamshaTypeReplacementOptions options)
    {
        ReplaceRole(options, typeof(TRole));
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceUser<TUser>(this RamshaTypeReplacementOptions options)
    {
        ReplaceUser(options, typeof(TUser));
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceRole(this RamshaTypeReplacementOptions options, Type roleType)
    {
        options.Replace(typeof(RamshaRoleTypeTypedKey), roleType);
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceUser(this RamshaTypeReplacementOptions options, Type userType)
    {
        options.Replace(typeof(RamshaUserTypeTypedKey), userType);
        return options;
    }



    private static RamshaTypeReplacementOptions ReplaceUserRole<TUserRole>(this RamshaTypeReplacementOptions options)
    {
        options.Replace(typeof(RamshaUserRoleTypeTypedKey), typeof(TUserRole));
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceUserClaim<TUserClaim>(this RamshaTypeReplacementOptions options)
    {
        options.Replace(typeof(RamshaUserClaimTypeTypedKey), typeof(TUserClaim));
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceUserLogin<TUserLogin>(this RamshaTypeReplacementOptions options)
    {
        options.Replace(typeof(RamshaUserLoginTypeTypedKey), typeof(TUserLogin));
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceUserToken<TUserToken>(this RamshaTypeReplacementOptions options)
    {
        options.Replace(typeof(RamshaUserTokenTypeTypedKey), typeof(TUserToken));
        return options;
    }

    private static RamshaTypeReplacementOptions ReplaceRoleClaim<TRoleClaim>(this RamshaTypeReplacementOptions options)
    {
        options.Replace(typeof(RamshaRoleClaimTypeTypedKey), typeof(TRoleClaim));
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

    private static Type GetBaseUserRoleType(Type idType)
    {
        return typeof(RamshaIdentityUserRole<>).MakeGenericType(idType);
    }

    private static Type GetBaseUserClaimType(Type idType)
    {
        return typeof(RamshaIdentityUserClaim<>).MakeGenericType(idType);
    }

    private static Type GetBaseUserTokenType(Type idType)
    {
        return typeof(RamshaIdentityUserToken<>).MakeGenericType(idType);
    }

    private static Type GetBaseUserLoginType(Type idType)
    {
        return typeof(RamshaIdentityUserLogin<>).MakeGenericType(idType);
    }

    private static Type GetBaseRoleClaimType(Type idType)
    {
        return typeof(RamshaIdentityRoleClaim<>).MakeGenericType(idType);
    }


}

internal sealed class RamshaUserTypeTypedKey : IRamshaTypedKey { }
internal sealed class RamshaRoleTypeTypedKey : IRamshaTypedKey { }
internal sealed class RamshaUserClaimTypeTypedKey : IRamshaTypedKey { }
internal sealed class RamshaUserRoleTypeTypedKey : IRamshaTypedKey { }
internal sealed class RamshaUserLoginTypeTypedKey : IRamshaTypedKey { }
internal sealed class RamshaUserTokenTypeTypedKey : IRamshaTypedKey { }
internal sealed class RamshaRoleClaimTypeTypedKey : IRamshaTypedKey { }
