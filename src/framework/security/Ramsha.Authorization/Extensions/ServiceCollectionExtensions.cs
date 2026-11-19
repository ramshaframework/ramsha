using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ramsha.Authorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {

        services.AddTransient<IPermissionDefinitionManager, PermissionDefinitionManager>();

        // register definition stores 
        services.AddSingleton<InMemoryPermissionDefinitionStore>();
        services.AddTransient<IPermissionDefinitionStore>(p => p.GetRequiredService<InMemoryPermissionDefinitionStore>());




        services.AddSingleton<IPermissionDefinitionContext, PermissionDefinitionContext>();

        services.AddSingleton<InMemoryPermissionStore>();
        services.TryAddTransient<IPermissionStore>(p => p.GetRequiredService<InMemoryPermissionStore>());

        services.AddScoped<IPermissionChecker, PermissionChecker>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, RamshaAuthorizationPolicyProvider>();

        services.AddScoped<PermissionProviderResolversManager>();


        var permissionResolverInterfaceType = typeof(IPermissionValueResolver);

        var permissionResolvers = RamshaTypeHelpers.GetImplementationTypes<AuthorizationModule>(permissionResolverInterfaceType);
        foreach (var resolver in permissionResolvers)
        {
            services.AddTransient(permissionResolverInterfaceType, resolver);
            services.AddTransient(resolver);

        }






        var definitionProviderInterfaceType = typeof(IPermissionDefinitionProvider);
        var definitionProviderTypes = RamshaTypeHelpers.GetImplementationTypes<AuthorizationModule>(definitionProviderInterfaceType);
        foreach (var provider in definitionProviderTypes)
        {
            services.AddTransient(provider);

            services.AddTransient(definitionProviderInterfaceType, provider);
        }


        return services;
    }
}
