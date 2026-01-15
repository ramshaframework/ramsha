

using Ramsha.JwtAuth.Domain;
using Ramsha.JwtAuth.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthPersistenceServices(this IServiceCollection services)
    {
        services
       .AddRamshaDbContext<IRamshaJwtAuthDbContext, RamshaJwtAuthDbContext>(options =>
       {
           options.AddRepository<RamshaRefreshToken, IRefreshTokenRepository, EFCoreRefreshTokenRepository>();
       });

        return services;
    }
}
