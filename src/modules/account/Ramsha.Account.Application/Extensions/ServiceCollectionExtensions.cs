
using Ramsha;
using Ramsha.Account.Application;


namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccountApplicationServices(this IServiceCollection services)
    {
        var typesOptions = services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        var serviceTypes = typesOptions.GetAccountServiceOrBase();
        services.AddRamshaService(serviceTypes.ImplementationType, serviceTypes.InterfaceType);

        return services;
    }
}
