
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Ramsha.Localization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRamshaLocalizationServices(this IServiceCollection services)
    {
        services.Configure<RamshaLocalizationOptions>(o => { });

        services.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, RamshaStringLocalizerFactory>());
        services.AddTransient(typeof(IStringLocalizer<>), typeof(RamshaStringLocalizer<>));



        return services;
    }
}
