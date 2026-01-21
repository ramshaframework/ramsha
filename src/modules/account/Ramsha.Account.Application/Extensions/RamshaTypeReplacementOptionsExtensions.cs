using Ramsha.Account.Contracts;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;

namespace Ramsha.Account.Application;

public static class RamshaTypeReplacementOptionsExtensions
{
    public static (Type InterfaceType, Type ImplementationType) GetAccountServiceOrBase(this RamshaTypeReplacementOptions options)
    {
        var baseService = GetBaseAccountServiceType(options);
        return (options.GetOrBase(baseService.InterfaceType), options.GetOrBase(baseService.ImplementationType));
    }


    public static RamshaTypeReplacementOptions ReplaceAccountService<TService>(this RamshaTypeReplacementOptions options)
    where TService : IRamshaAccountServiceBase
    {
        var service = GetBaseAccountServiceType(options);
        options.ForceReplace(service.InterfaceType, typeof(TService));
        options.ForceReplace(service.ImplementationType, typeof(TService));

        return options;
    }


    private static (Type InterfaceType, Type ImplementationType) GetBaseAccountServiceType(RamshaTypeReplacementOptions options)
    {
        var entitiesTypes = options.GetRamshaIdentityEntitiesTypes();
        var keyType = options.GetIdentityIdOrBase();

        var implementationType = typeof(RamshaAccountService<,,,,,,,,>).MakeGenericType(
            entitiesTypes.UserType,
            entitiesTypes.RoleType,
            keyType,
            entitiesTypes.UserRoleType,
            entitiesTypes.RoleClaimType,
            entitiesTypes.UserClaimType,
            entitiesTypes.UserLoginType,
            entitiesTypes.UserTokenType,
           options.GetOrBase<RamshaRegisterDto>()

        );
        var interfaceType = typeof(IRamshaAccountService<>)
        .MakeGenericType(
           options.GetOrBase<RamshaRegisterDto>()
            );

        return (interfaceType, implementationType);
    }


}
