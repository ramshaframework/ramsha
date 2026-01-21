
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
                typesOptions.GetOrSelf<RamshaJwtAuthenticateRequest>(),
                typesOptions.GetOrSelf<RamshaJwtRefreshRequest>(),
                typesOptions.GetOrSelf<RamshaJwtAuthResponse>()
            )
            );
    }
}
