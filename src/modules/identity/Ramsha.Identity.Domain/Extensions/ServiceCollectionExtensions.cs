using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRamshaIdentityDomainServices(this IServiceCollection services)
    {
        var typesOptions = services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();

        var entitiesTypes = typesOptions.GetRamshaIdentityEntitiesTypes();
        var keyType = typesOptions.GetIdentityIdOrBase();

        // register userManager
        var ramshaUserManagerType = typeof(RamshaIdentityUserManager<,,,,,,,>).MakeGenericType(entitiesTypes.UserType, entitiesTypes.RoleType, keyType, entitiesTypes.UserRoleType, entitiesTypes.RoleClaimType, entitiesTypes.UserClaimType, entitiesTypes.UserLoginType, entitiesTypes.UserTokenType);

        services.TryAddScoped(ramshaUserManagerType);
        services.TryAddScoped(typeof(RamshaIdentityUserManager<>));
        services.TryAddScoped(typeof(RamshaIdentityUserManager<,>));

        services.TryAddScoped(typeof(UserManager<>).MakeGenericType(entitiesTypes.UserType), provider => provider.GetService(ramshaUserManagerType));

        // register roleManager
        var ramshaRoleManagerType = typeof(RamshaIdentityRoleManager<,,,>).MakeGenericType(entitiesTypes.RoleType, keyType, entitiesTypes.UserRoleType, entitiesTypes.RoleClaimType);
        services.TryAddScoped(ramshaRoleManagerType);
        services.TryAddScoped(typeof(RamshaIdentityRoleManager<>));
        services.TryAddScoped(typeof(RoleManager<>).MakeGenericType(entitiesTypes.RoleType), provider => provider.GetService(ramshaRoleManagerType));

        return services;

    }
}
