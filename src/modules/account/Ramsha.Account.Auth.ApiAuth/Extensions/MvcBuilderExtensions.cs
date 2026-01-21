
using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;


namespace Ramsha.Account.Auth.ApiAuth;

public static class MvcBuilderExtensions
{
    public static void AddAccountApiAuthGenericControllers(this IMvcBuilder builder)
    {
        var typesOptions = builder.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        builder.AddGenericControllers(
            typeof(AccountController<,,,>)
            .MakeGenericType(
             typesOptions.GetUserTypeOrBase(),
             typesOptions.GetIdentityIdOrBase(),
             typesOptions.GetOrBase<RamshaLoginRequest>(),
             typesOptions.GetOrBase<RamshaLoginResponse>()
             )
            );
    }
}
