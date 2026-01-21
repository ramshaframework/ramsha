using Ramsha.Identity.Contracts;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Application;

public static class RamshaTypeReplacementOptionsExtensions
{
    public static (Type InterfaceType, Type ImplementationType) GetIdentityUserServiceOrBase(this RamshaTypeReplacementOptions options)
    {
        var baseService = GetBaseUserServiceType(options);
        return (baseService.InterfaceType, options.GetOrSelf(baseService.ImplementationType));
    }

    public static (Type InterfaceType, Type ImplementationType) GetIdentityRoleServiceOrBase(this RamshaTypeReplacementOptions options)
    {
        var baseService = GetBaseRoleServiceType(options);
        return (baseService.InterfaceType, options.GetOrSelf(baseService.ImplementationType));
    }

    public static RamshaTypeReplacementOptions ReplaceIdentityUserService<TService>(this RamshaTypeReplacementOptions options)
    where TService : IRamshaIdentityUserServiceBase
    {
        var service = GetBaseUserServiceType(options);
        options.ReplaceBase(service.ImplementationType, typeof(TService));

        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceIdentityRoleService<TService>(RamshaTypeReplacementOptions options)
    where TService : IRamshaIdentityRoleServiceBase
    {
        var service = GetBaseRoleServiceType(options);
        options.ReplaceBase(service.ImplementationType, typeof(TService));
        return options;
    }

    private static (Type InterfaceType, Type ImplementationType) GetBaseUserServiceType(this RamshaTypeReplacementOptions options)
    {
        var idType = options.GetIdentityIdOrBase();
        var userDto = options.GetOrSelf<RamshaIdentityUserDto>();
        var updateDto = options.GetOrSelf<UpdateRamshaIdentityUserDto>();
        var createUserDto = options.GetOrSelf<CreateRamshaIdentityUserDto>();
        var interfaceType = typeof(IRamshaIdentityUserService<,,,>)
           .MakeGenericType(
           userDto,
           createUserDto,
           updateDto,
           idType
          );

        var implementationType = typeof(RamshaIdentityUserService<,,,,,,,,,,>).MakeGenericType(
            options.GetUserTypeOrBase(),
            options.GetRoleTypeOrBase(),
            idType,
            options.GetUserRoleTypeOrBase(),
            options.GetRoleClaimTypeOrBase(),
            options.GetUserClaimTypeOrBase(),
            options.GetUserLoginTypeOrBase(),
            options.GetUserTokenTypeOrBase(),
            userDto,
            createUserDto,
            updateDto
        );

        return (interfaceType, implementationType);
    }

    private static (Type InterfaceType, Type ImplementationType) GetBaseRoleServiceType(this RamshaTypeReplacementOptions options)
    {

        var idType = options.GetIdentityIdOrBase();
        var roleDto = options.GetOrSelf<RamshaIdentityRoleDto>();
        var updateRoleDto = options.GetOrSelf<UpdateRamshaIdentityRoleDto>();
        var createRoleDto = options.GetOrSelf<CreateRamshaIdentityRoleDto>();
        var implementationType = typeof(RamshaIdentityRoleService<,,,,,,>).MakeGenericType(
            options.GetRoleTypeOrBase(),
            idType,
            options.GetUserRoleTypeOrBase(),
            options.GetRoleClaimTypeOrBase(),
            roleDto,
            createRoleDto,
           updateRoleDto);

        var interfaceType = typeof(IRamshaIdentityRoleService<,,,>)
       .MakeGenericType(
            roleDto,
            createRoleDto,
           updateRoleDto,
           idType
           );


        return (interfaceType, implementationType);
    }


}
