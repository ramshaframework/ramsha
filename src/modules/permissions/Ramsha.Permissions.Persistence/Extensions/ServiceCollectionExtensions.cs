
using Ramsha.Permissions.Domain;
using Ramsha.Permissions.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionsPersistenceServices(this IServiceCollection services)
    {
        services.AddRamshaDbContext<IPermissionsDbContext, PermissionsDbContext>(options =>
        {
            options.AddRepository<Permission, IPermissionRepository, EFCorePermissionRepository>();
        });

        return services;
    }
}
