using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Identity.Contracts;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRamshaIdentityApplicationServices(this IServiceCollection services)
    {
        var typesOptions = services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        var userService = typesOptions.GetIdentityUserServiceOrBase();

        services.AddRamshaService(userService.ImplementationType, userService.InterfaceType);

        var roleService = typesOptions.GetIdentityRoleServiceOrBase();

        services.AddRamshaService(roleService.ImplementationType, roleService.InterfaceType);


        return services;
    }
}
