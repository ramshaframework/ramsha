using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Contracts;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Api;

public static class MvcBuilderExtensions
{
    public static void AddIdentityGenericControllers(this IMvcBuilder builder)
    {
        var typesOptions = builder.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        var keyType = typesOptions.GetIdentityIdOrBase();

        builder.AddGenericControllers(
            typeof(RamshaIdentityRoleController<,,,>).MakeGenericType(typesOptions.GetOrSelf<RamshaIdentityRoleDto>(), typesOptions.GetOrSelf<CreateRamshaIdentityRoleDto>(), typesOptions.GetOrSelf<UpdateRamshaIdentityRoleDto>(), keyType),
            typeof(RamshaIdentityUserController<,,,>).MakeGenericType(typesOptions.GetOrSelf<RamshaIdentityUserDto>(), typesOptions.GetOrSelf<CreateRamshaIdentityUserDto>(), typesOptions.GetOrSelf<UpdateRamshaIdentityUserDto>(), keyType)
            );
    }
}
