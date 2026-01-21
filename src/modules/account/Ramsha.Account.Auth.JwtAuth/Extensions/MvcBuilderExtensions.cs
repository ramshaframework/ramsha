
using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;

namespace Ramsha.Account.Auth.JwtAuth;

public static class MvcBuilderExtensions
{
    public static void AddJwtAuthGenericControllers(this IMvcBuilder builder)
    {
        var typesOptions = builder.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        builder.AddGenericControllers(
            typeof(AccountController<,,,,>)
            .MakeGenericType(
                typesOptions.GetUserTypeOrBase(),
                typesOptions.GetIdentityIdOrBase(),
                typesOptions.GetOrBase<RamshaJwtAuthenticateRequest>(),
                typesOptions.GetOrBase<RamshaJwtRefreshRequest>(),
                typesOptions.GetOrBase<RamshaJwtAuthResponse>()
            )
            );
    }
}
