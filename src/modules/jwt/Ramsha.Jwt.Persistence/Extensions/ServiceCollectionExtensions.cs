

using Ramsha.Jwt.Domain;
using Ramsha.Jwt.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtPersistenceServices(this IServiceCollection services)
    {
        services
       .AddRamshaDbContext<IRamshaJwtDbContext, RamshaJwtDbContext>(options =>
       {
           options.AddRepository<RamshaRefreshToken, IRefreshTokenRepository, EFCoreRefreshTokenRepository>();
       });

        return services;
    }
}
