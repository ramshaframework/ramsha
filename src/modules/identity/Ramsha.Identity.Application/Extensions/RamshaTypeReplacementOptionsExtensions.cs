using Ramsha.Identity.Contracts;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Application;

public static class RamshaTypeReplacementOptionsExtensions
{
    public static (Type InterfaceType, Type ImplementationType) GetUserServiceOrBase(this RamshaTypeReplacementOptions options)
    {
        var baseService = GetBaseUserServiceType(options);
        return (options.GetOrBase(baseService.InterfaceType), options.GetOrBase(baseService.ImplementationType));
    }

    public static (Type InterfaceType, Type ImplementationType) GetRoleServiceOrBase(this RamshaTypeReplacementOptions options)
    {
        var baseService = GetBaseRoleServiceType(options);
        return (options.GetOrBase(baseService.InterfaceType), options.GetOrBase(baseService.ImplementationType));
    }

    public static RamshaTypeReplacementOptions ReplaceUserService<TService>(this RamshaTypeReplacementOptions options)
    where TService : IRamshaIdentityUserServiceBase
    {
        var service = GetBaseUserServiceType(options);
        options.ForceReplace(service.InterfaceType, typeof(TService));
        options.ForceReplace(service.ImplementationType, typeof(TService));

        return options;
    }

    public static RamshaTypeReplacementOptions ReplaceRoleService<TService>(RamshaTypeReplacementOptions options)
    where TService : IRamshaIdentityRoleServiceBase
    {
        var service = GetBaseRoleServiceType(options);
        options.ForceReplace(service.InterfaceType, typeof(TService));
        options.ForceReplace(service.ImplementationType, typeof(TService));
        return options;
    }

    private static (Type InterfaceType, Type ImplementationType) GetBaseUserServiceType(RamshaTypeReplacementOptions options)
    {
        var idType = options.GetIdentityIdOrBase();
        var userDto = options.GetOrBase<RamshaIdentityUserDto>();
        var updateDto = options.GetOrBase<UpdateRamshaIdentityUserDto>();
        var createUserDto = options.GetOrBase<CreateRamshaIdentityUserDto>();
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

    private static (Type InterfaceType, Type ImplementationType) GetBaseRoleServiceType(RamshaTypeReplacementOptions options)
    {

        var idType = options.GetIdentityIdOrBase();
        var roleDto = options.GetOrBase<RamshaIdentityRoleDto>();
        var updateRoleDto = options.GetOrBase<UpdateRamshaIdentityRoleDto>();
        var createRoleDto = options.GetOrBase<CreateRamshaIdentityRoleDto>();
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
