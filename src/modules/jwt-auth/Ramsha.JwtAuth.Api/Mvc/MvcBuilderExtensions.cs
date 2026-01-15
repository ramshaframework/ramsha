
using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore.Mvc;
using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.Api;

public static class MvcBuilderExtensions
{
    public static void AddJwtAuthGenericControllers(this IMvcBuilder builder)
    {
        var typesOptions = builder.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        builder.AddGenericControllers(
            typeof(RamshaJwtAuthController<,,>)
            .MakeGenericType(
                typesOptions.GetOrBase<RamshaJwtAuthenticateRequest>(),
                typesOptions.GetOrBase<RamshaJwtRefreshRequest>(),
                typesOptions.GetOrBase<RamshaJwtAuthResponse>()
            )
            );
    }
}
