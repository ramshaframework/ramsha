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
            typeof(RamshaIdentityRoleController<,,,>).MakeGenericType(typesOptions.GetOrBase<RamshaIdentityRoleDto>(), typesOptions.GetOrBase<CreateRamshaIdentityRoleDto>(), typesOptions.GetOrBase<UpdateRamshaIdentityRoleDto>(), keyType),
            typeof(RamshaIdentityUserController<,,,>).MakeGenericType(typesOptions.GetOrBase<RamshaIdentityUserDto>(), typesOptions.GetOrBase<CreateRamshaIdentityUserDto>(), typesOptions.GetOrBase<UpdateRamshaIdentityUserDto>(), keyType)
            );
    }
}
